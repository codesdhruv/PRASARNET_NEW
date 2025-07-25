using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using PRASARNET.Repositories.Interfaces;

namespace PRASARNET.Authorization
{
    public class CustomClaimsTransformer : IClaimsTransformation
    {
        private readonly IUserDataRepository _userDataRepo;

        public CustomClaimsTransformer(IUserDataRepository userDataRepo)
        {
            _userDataRepo = userDataRepo;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = (ClaimsIdentity)principal.Identity!;
            var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value ?? principal.FindFirst("email")?.Value;
            var username = principal.Identity?.Name;            
            if (string.IsNullOrEmpty(username)) return principal;

            // Check if claims already added
            if (identity.HasClaim("transformed", "true"))
                return principal;

            var roles = await _userDataRepo.GetRolesForUserAsync(userEmail);
            var permissions = await _userDataRepo.GetPermissionsForUserAsync(userEmail);

            foreach (var role in roles)
                identity.AddClaim(new Claim(ClaimTypes.Role, role));

            foreach (var permission in permissions)
                identity.AddClaim(new Claim("permission", permission));

            // Add marker claim to prevent repeated work
            identity.AddClaim(new Claim("transformed", "true"));

            return principal;
        }


    }
}
