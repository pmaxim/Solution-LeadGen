using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace BlazorWeb.Client.Infrastructure
{
    //https://github.com/javiercn/BlazorAuthRoles/blob/master/Client/Program.cs
    public class RolesClaimsPrincipalFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        public RolesClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor) : base(accessor)
        {
        }

        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {
            var user = await base.CreateUserAsync(account, options);
            if (user.Identity is not { IsAuthenticated: true }) return user;
            var identity = (ClaimsIdentity)user.Identity;
            var roleClaims = identity.FindAll(identity.RoleClaimType);
            var existingClaims = roleClaims.ToList();
            if (!existingClaims.Any()) return user;

            for (var i = 0; i < existingClaims.Count; i++)
            {
                identity.RemoveClaim(existingClaims[i]);
            }

            var rolesElem = account.AdditionalProperties[identity.RoleClaimType];
            if (rolesElem is not JsonElement roles) return user;

            if (roles.ValueKind == JsonValueKind.Array)
            {
                foreach (var role in roles.EnumerateArray())
                {
                    identity.AddClaim(new Claim(options.RoleClaim, role.GetString() ?? string.Empty));
                }
            }
            else
            {
                identity.AddClaim(new Claim(options.RoleClaim, roles.GetString() ?? string.Empty));
            }

            return user;
        }
    }
}