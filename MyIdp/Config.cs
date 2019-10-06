// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace MyIdp
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("restapi", "My RESTful API")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {              
                // MVC client using hybrid flow
                new Client
                {
                    ClientId = "mvcclient",
                    ClientName = "MVC 客户端",

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    RedirectUris = { "https://localhost:7001/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:7001/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:7001/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    //AllowedScopes = { "openid", "profile", "api1" }
                    //AllowedScopes = { "openid"}
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "restapi"
                    }
                },

                // Vue client using code flow + pkce
                new Client
                {
                    ClientId = "vuejsclient",//必须对应前端项目中的client_id属性的值
                    ClientName = "Vue Client",
                    ClientUri = "http://localhost:8080",//前端项目地址

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    AccessTokenLifetime = 180,

                    //RequirePkce = true,
                    //RequireClientSecret = false,

                    RedirectUris =
                    {                        
                        "http://localhost:8080/callback.html",
                        "http://localhost:8080/silent.html",
                        "http://localhost:8080/popup.html",
                        "http://localhost:8080/freeuser"
                    },

                    PostLogoutRedirectUris = { "http://localhost:8080/"},
                    AllowedCorsOrigins = { "http://localhost:8080" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "restapi"
                    }
                }
            };
        }
    }
}