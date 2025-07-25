using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using PRASARNET.Services;
using System.Configuration;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PRASARNET.Controllers
{
    public class AccountController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IConfiguration _configuration;

        public AccountController(ISessionService sessionService, IConfiguration configuration)
        {
            _sessionService = sessionService;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = Url.Action("Index", "Home") }, "oidc");
        }

        public IActionResult Logout()
        {
            _sessionService.ClearSession();
            return SignOut(new AuthenticationProperties { RedirectUri = Url.Action("Index", "Account", new { area = "" }) }, "Cookies", "oidc");
        }

        public async Task<IActionResult> AllDeviceLogout()
        {
            var authority = _configuration["Authentication:Keycloak:Authority"];
            var idToken = TempData["id_token"] as string;
            var encodedIdToken = WebUtility.UrlEncode(idToken);

            //var logoutUri = $"http://localhost:8080/realms/keycloak-auth-prasarnet/protocol/openid-connect/logout?id_token_hint={encodedIdToken}";
            // var postLogoutRedirectUri = WebUtility.UrlEncode("http://localhost:5800/Account/Index");
            //var postLogoutRedirectUri = WebUtility.UrlEncode("https://betaprasarnet.prasarbharati.org/Account/Index");
           
            var postLogoutRedirectUri = _configuration["Authentication:Keycloak:PostLogoutRedirectUri"];
            var encodedPostLogoutRedirectUri = WebUtility.UrlEncode(postLogoutRedirectUri);
            var logoutUri = $"{authority}/protocol/openid-connect/logout" +
                         $"?id_token_hint={encodedIdToken}" +
                         $"&post_logout_redirect_uri={encodedPostLogoutRedirectUri}";

            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
            return Redirect(logoutUri);
        }

        [Route("signout-callback-oidc")]
        public IActionResult SignOutCallback()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied(string token)
        {
            TempData["id_token"] = token;
            ViewData["Message"] = "You are not authorized to access this resource.";
            return View();
        }
        public IActionResult NoAccess(string error)
        {
            ViewBag.ErrorMessage = error ?? "NoAccess Method called.";
            return View();
        }

        public IActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

    }
}
