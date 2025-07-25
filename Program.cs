using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using PRASARNET.Services;
using System.Security.Claims;
using NuGet.Packaging;
using System.Net;
using PRASARNET.Extensions;
using PRASARNET.Repositories.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/error-.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});


builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SessionService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "oidc";
    options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
 .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
 {
     options.Cookie.SameSite = SameSiteMode.None;  // Important!
     options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Always use HTTPS
     options.AccessDeniedPath = "/Account/AccessDenied";
 })
.AddOpenIdConnect("oidc", options =>
{
    var config = builder.Configuration.GetSection("Authentication:Keycloak");

    options.Authority = config["Authority"];
    options.RequireHttpsMetadata = false; // This line disables the HTTPS check
    options.ClientId = config["ClientId"];
    options.ClientSecret = config["ClientSecret"];
    options.ResponseType = "code";
    options.SaveTokens = true;

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.GetClaimsFromUserInfoEndpoint = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        NameClaimType = "preferred_username",
        
    };

    options.Events = new OpenIdConnectEvents
    {
        OnRemoteFailure = context =>
        {
            var errorMessage = context.Failure?.Message ?? "Remote authentication failed.(OnRemoteFailure)";
            var encodedMessage = WebUtility.UrlEncode(errorMessage);
            context.Response.Redirect($"/Account/NoAccess?error={encodedMessage + "Remote authentication failed"}");
            context.HandleResponse();
            return Task.CompletedTask;
        },
        OnAuthorizationCodeReceived = context =>
        {
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var exceptionMessage = context.Exception?.Message ?? "Unknown error(OnAuthenticationFailed)";
            var encodedMessage = WebUtility.UrlEncode(exceptionMessage);
            context.Response.Redirect($"/Account/NoAccess?error={encodedMessage + "OnAuthentication Failed"}");
            context.HandleResponse();
            return Task.CompletedTask;
        },
        OnTokenValidated = async context =>
        {
            List<string> allowedApps = new List<string>();
            var userDataRepo = context.HttpContext.RequestServices.GetRequiredService<IUserDataRepository>();
            var userEmail = context.Principal?.FindFirst(ClaimTypes.Email)?.Value;
            
            var employee = await userDataRepo.FetchEmployeeByEmailAsync(userEmail);

            foreach (var claim in context.Principal.Claims)
            {
                if (claim.Type == "allowed_app")
                {
                    allowedApps.AddRange(claim.Value);
                }
                //Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }
            allowedApps.AddRange("prasarnet");  //Enforcing Application permisssion for development Environment
            string[] allowedAppArray = allowedApps.ToArray();

            if (!allowedAppArray.Contains("prasarnet") || employee == null)
            {
                context.HandleResponse(); // stop default pipeline
                var idTokens = context.TokenEndpointResponse?.IdToken;
                var encodedToken = WebUtility.UrlEncode(idTokens);
                var redirectUrl = $"/Account/AccessDenied?token={encodedToken}";
                context.Response.Redirect(redirectUrl);
                return;
            }
            else
            {
                var sessionService = context.HttpContext.RequestServices.GetRequiredService<ISessionService>();
                await sessionService.SetEmployeeSessionAsync(employee);

                var sid = context.Principal.FindFirst("sid")?.Value;
                var userId = context.Principal.Identity?.Name;
                if (!string.IsNullOrEmpty(sid) && !string.IsNullOrEmpty(userId))
                {
                    var sidStore = context.HttpContext.RequestServices.GetRequiredService<ISidStore>();
                    await sidStore.SaveAsync(sid, userId);
                }
            }

        },
    };
});

// Register Services in ServiceCollectionExtensions
builder.Services.AddApplicationServices();
builder.Services.AddRepositories();
builder.Services.AddAuthorizationPolicies();
builder.Services.AddCustomSessionAndSidStore();

var app = builder.Build();


app.UseHttpsRedirection();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/html";

            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;

            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exception, "Unhandled exception occurred");

            // Show the exception on the page
            await context.Response.WriteAsync($@"
                <html>
                <body style='font-family:Arial; padding:20px; color: red;'>
                    <h1>Oops! An error occurred.</h1>
                    <pre>{exception?.Message}</pre>
                    <pre>{exception?.StackTrace}</pre>
                </body>
                </html>");
        });
    });
}

app.UseForwardedHeaders();
app.UseRouting();         
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    Secure = CookieSecurePolicy.Always
});
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<SidLogoutCheckMiddleware>();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
