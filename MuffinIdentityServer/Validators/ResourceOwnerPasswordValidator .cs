using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using MuffinIdentityServer.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MuffinIdentityServer
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly RepositoryContext _repositoryContext;

        public ResourceOwnerPasswordValidator(RepositoryContext repositoryContext) => _repositoryContext = repositoryContext;

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UserName) ||
                string.IsNullOrWhiteSpace(context.Password))
            {
                context.Result = new GrantValidationResult
                    (TokenRequestErrors.InvalidGrant, "Invalid username or password.");
                return Task.CompletedTask;
            }

            var userProfile = _repositoryContext.UserProfiles
                .SingleOrDefault(i => i.Username == context.UserName && i.IsActive);

            if (userProfile == null)
            {
                context.Result = new GrantValidationResult
                    (TokenRequestErrors.InvalidGrant, "User does not exist.");
                return Task.CompletedTask;
            }
            else
            {
                userProfile.IsTotpValid = false;
                _repositoryContext.SaveChanges();
            }

            var salt = CryptoHelper.ConvertStringToByteArray(userProfile.Salt);
            var hashPassword = CryptoHelper.HashPassword(context.Password, salt);

            if (hashPassword != userProfile.Password)
            {
                context.Result = new GrantValidationResult
                    (TokenRequestErrors.InvalidGrant, "Invalid username or password.");
                return Task.CompletedTask;
            }

            context.Result = new GrantValidationResult(
                        subject: userProfile.Id.ToString(),
                        authenticationMethod: "custom",
                        claims: GetUserClaims());

            return Task.CompletedTask;
        }

        public static Claim[] GetUserClaims()
        {
            var issued = DateTimeOffset.Now.ToUnixTimeSeconds();

            return new Claim[]
            {
                new Claim(JwtClaimTypes.Subject, Guid.NewGuid().ToString()),
                new Claim(JwtClaimTypes.AuthenticationTime, issued.ToString()),
                new Claim(JwtClaimTypes.IdentityProvider, "localhost")
            };
        }
    }
}