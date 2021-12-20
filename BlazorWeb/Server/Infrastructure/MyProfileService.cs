using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;

namespace BlazorWeb.Server.Infrastructure
{
    //https://stackoverflow.com/questions/56282355/adding-claims-to-identityserver-setup-by-addidentityserver
    public class MyProfileService : IProfileService
    {
        public MyProfileService() { }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var roleClaims = context.Subject.FindAll(JwtClaimTypes.Role);
            context.IssuedClaims.AddRange(roleClaims);

            var nameClaims = context.Subject.FindAll(JwtClaimTypes.Name);
            context.IssuedClaims.AddRange(nameClaims);

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            // await base.IsActiveAsync(context);
            return Task.CompletedTask;
        }
    }
}
