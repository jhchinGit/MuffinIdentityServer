using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace MuffinIdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiResource> GetApiResources =>
            new List<ApiResource>
            {
                new ApiResource("api1", "My API"),
                new ApiResource("postman_api", "Postman Test Resource")
            };

        public static IEnumerable<ApiScope> GetApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api1", "My API"),
                new ApiScope("postman_api", "Postman Test Resource")
            };

        public static IEnumerable<Client> GetClients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    AllowAccessTokensViaBrowser = true,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                },
                new Client
                {
                    ClientId = "postman-api",
                    ClientName = "Postman Test Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    RedirectUris = { "https://www.getpostman.com/oauth2/callback" },
                    PostLogoutRedirectUris = { "https://www.getpostman.com" },
                    AllowedCorsOrigins = { "https://www.getpostman.com" },
                    EnableLocalLogin = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "postman_api"
                    },
                    ClientSecrets = new List<Secret>() { new Secret("SomeValue".Sha256()) }
                }
            };
    }
}