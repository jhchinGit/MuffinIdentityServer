using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using MuffinIdentityServer.Services;
using MuffinIdentityServer.Totp;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MuffinIdentityServer
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly ITotpSetupGenerator _totpSetupGenerator;
        private readonly ITotpGenerator _totpGenerator;
        private readonly ITotpValidator _totpValidator;

        public ResourceOwnerPasswordValidator(RepositoryContext repositoryContext,
            ITotpSetupGenerator totpSetupGenerator,
            ITotpGenerator totpGenerator,
            ITotpValidator totpValidator)
        {
            _repositoryContext = repositoryContext;
            _totpSetupGenerator = totpSetupGenerator;
            _totpGenerator = totpGenerator;
            _totpValidator = totpValidator;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UserName) ||
                string.IsNullOrWhiteSpace(context.Password))
            {
                context.Result = new GrantValidationResult
                    (TokenRequestErrors.InvalidGrant, "Invalid username or password.");
                return Task.CompletedTask;
            }

            if (string.IsNullOrWhiteSpace(context.Request.Raw["totp"]) ||
                !int.TryParse(context.Request.Raw["totp"], out var currentTotp))
            {
                context.Result = new GrantValidationResult
                   (TokenRequestErrors.InvalidGrant, "Invalid authentication code");
                return Task.CompletedTask;
            }

            var setupGeneratorKey = _totpSetupGenerator.Generate("muffinsdnbhdwestworld");
            var isValidTotp = _totpValidator.IsValidTotp("muffinsdnbhdwestworld", currentTotp);

            if (!isValidTotp)
            {
                context.Result = new GrantValidationResult
                   (TokenRequestErrors.InvalidGrant, "Invalid authentication code");
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