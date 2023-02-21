// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;

namespace sso_spike.sts.adapters;

 public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api1", "My API")
            };

        public static IEnumerable<ApiResource> GetApiResources() =>
            new List<ApiResource> 
            { 
                new ApiResource("api1", "My API") 
                { 
                    Scopes = { "api1" } 
                } 
            };
        
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new()
                {
                    ClientName = "UI",
                    ClientId = "UI",
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = new List<string>{ "http://localhost:4200/#/signin-callback" },
                    RequirePkce = true,
                    AllowAccessTokensViaBrowser = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    AllowedCorsOrigins = { "http://localhost:4200" },
                    RequireClientSecret = false,
                    PostLogoutRedirectUris = new List<string> { "http://localhost:4200/#/signout-callback" },
                    RequireConsent = false,
                    AccessTokenLifetime = 600,
                    AlwaysIncludeUserClaimsInIdToken = true
                }
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                
            };
    }

public static class CustomClaimTypes
{
    public const string UserProfile = "UserProfile";
}