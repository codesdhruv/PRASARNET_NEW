using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using PRASARNET.Areas.PrasarNet.Services;
using PRASARNET.Areas.TRAM.Repositories;
using PRASARNET.Authorization;
using PRASARNET.Repositories;
using PRASARNET.Repositories.Interfaces;
using PRASARNET.Services;

namespace PRASARNET.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Add other Services
            services.AddScoped<CircularService>();

        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddSingleton<IUserDataRepository, UserDataRepository>();
            services.AddSingleton<ApplicantRepository>();
            services.AddSingleton<HandelApplicationRepository>();
            services.AddScoped<RolesRepository>();

            // Add other repositories
        }

        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IClaimsTransformation, CustomClaimsTransformer>();
        }

        public static void AddCustomSessionAndSidStore(this IServiceCollection services)
        {
            services.AddScoped<ISessionService, SessionService>();
            services.AddSingleton<ISidStore, InMemorySidStore>(); // replace with Redis for scale
        }
    }
}
