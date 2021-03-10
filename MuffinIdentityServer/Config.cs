using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;

namespace MuffinIdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiResource> GetApiResources() =>
            new List<ApiResource>
            {
                new ApiResource("muffinscopeapi", "Muffin Resource Api"),
                new ApiResource(IdentityServerConstants.StandardScopes.OfflineAccess, "Muffin Refresh Resource Api")
            };

        public static IEnumerable<ApiScope> GetApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("muffinscopeapi", "Muffin Scope Api"),
                new ApiScope(IdentityServerConstants.StandardScopes.OfflineAccess, "Muffin Refresh Scope Api"),
            };

        public static List<TestUser> GetUsers()
        => new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "1",
                Username = "alice",
                Password = "password"
            },
            new TestUser
            {
                SubjectId = "2",
                Username = "bob",
                Password = "password"
            }
        };

        public static IEnumerable<Client> GetClients =>
            new List<Client>
            {
                new Client
                {
                    ClientName = "Muffin Owner Flow",
                    ClientId = "muffin_owner_flow",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("muffinsecret".Sha512())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "muffinscopeapi"
                    },
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    AccessTokenLifetime = 500,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = 86400, // using default
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = { "http://localhost:8471" },
                },
            };
    }
}