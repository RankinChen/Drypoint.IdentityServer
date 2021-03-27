using Drypoint.IdentityServer.Hosting.Models;
using Drypoint.IdentityServer.Hosting.ToolKit;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Drypoint.IdentityServer.Hosting.Data.InitData
{
    public static class IdentityServerInitData
    {
        /// <summary>
        /// api资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new ApiResource[]
            {
                new ApiResource("Drypoint_Host_API", "Drypoint Host API(All)")
                {
                    Description="所有的API包括前端和后端",
                    UserClaims= { JwtClaimTypes.Name, JwtClaimTypes.Role },
                    ApiSecrets = { new Secret("Drypoint_Host_API_6E183983F7654289AE79077DDD28C3B4".Sha256()) }
                }
            };
        }

        /// <summary>
        /// 身份资源范围
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            //IdentityServer支持的一些标准OpenID Connect定义的范围
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(), //必须包含
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResources.Phone(),
                new IdentityResources.Email(),
                //自定义一个叫role的scope
                new IdentityResource {
                    Name =DrypointConsts.RolesScope,            //scope的名字
                    DisplayName="角色",                         //scope的显示名
                    UserClaims = new List<string> { JwtClaimTypes.Role }      //scope所包含的claim类型
                }
            };
        }

        /// <summary>
        /// 客户端
        /// </summary>
        public static IEnumerable<Client> GetClients()
        {
            return new[]{
                new Client{
                    ClientId= "code client",
                    ClientName= "NSwag Authorize用",
                    //hybrid混合模式 详见:IdentityServer4.Models.GrantTypes
                    AllowedGrantTypes= GrantTypes.Code,
                    AccessTokenType = AccessTokenType.Reference,
                    AllowAccessTokensViaBrowser=false,
                    //如果不需要显示否同意授权页面 这里就设置为false",
                    RequireConsent= false,
                    ClientSecrets ={
                        new Secret("code secret".Sha256())
                    },
                    //登录成功后返回的客户端地址
                    RedirectUris= {
                        "http://localhost:60000/swagger/oauth2-redirect.html"
                    },
                    //注销登录后返回的客户端地址
                    PostLogoutRedirectUris={
                        "http://localhost:60000/swagger/oauth2-redirect.html"
                    },
                    //详见:IdentityServerConstants.StandardScopes
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Phone,
                        "Drypoint_Host_API",
                        DrypointConsts.RolesScope
                    },
                    AlwaysIncludeUserClaimsInIdToken= true,
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 60 * 30
                },
                new Client{
                    ClientId="hybrid client",
                    ClientName= "Hybrid 客户端",
                    //允许跨域访问的地址（通常是前端页面直接访问授权）
                    AllowedCorsOrigins={ "http://localhost:7000" },
                    //hybrid混合模式 详见:IdentityServer4.Models.GrantTypes
                    AllowedGrantTypes=GrantTypes.Hybrid,
                    AccessTokenType = AccessTokenType.Reference, //默认值为 JWT   Reference方式需要API提供身份认证
                    AllowAccessTokensViaBrowser= false, //为True可能会导致登录过后 关闭浏览器 再打开不会唤起登录页面
                    //如果不需要显示否同意授权页面 这里就设置为false": null,
                    RequireConsent=true,
                    ClientSecrets ={
                        new Secret("hybrid secret".Sha256())
                    },
                    //登录成功后返回的客户端地址
                    RedirectUris={
                        "http://localhost:7000/signin-oidc" 
                        // "https://localhost:60000/swagger/index.html"  
                    },
                    //注销登录后返回的客户端地址
                    PostLogoutRedirectUris={
                        "http://localhost:7000/signout-callback-oidc"
                            //"https://localhost:60000/swagger/index.html"
                    },
                    AlwaysIncludeUserClaimsInIdToken= true,
                    AllowOfflineAccess= true,
                    //详见:IdentityServerConstants.StandardScopes
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Phone,
                        "Drypoint_Host_API",
                        DrypointConsts.RolesScope
                    },
                },
                new Client{
                    ClientId="angular client",
                    ClientName= "Angular 客户端",
                    ClientUri= "http://localhost:4200",
                    //允许跨域访问的地址（通常是前端页面直接访问授权）
                    AllowedCorsOrigins= {"http://localhost:4200" },
                    //hybrid混合模式 详见:IdentityServer4.Models.GrantTypes
                    AllowedGrantTypes=GrantTypes.HybridAndClientCredentials,
                    AccessTokenType = AccessTokenType.Reference, //默认值为 JWT   Reference方式需要API提供身份认证
                    AllowAccessTokensViaBrowser= true,
                    //如果不需要显示否同意授权页面 这里就设置为false",
                    RequireConsent= false,
                    ClientSecrets ={
                        new Secret("angular client".Sha256())
                    },
                    //登录成功后返回的客户端地址
                    RedirectUris={
                        "http://localhost:4200/signin-oidc",
                        "http://localhost:4200/redirect-silentrenew"
                    },
                    //注销登录后返回的客户端地址
                    PostLogoutRedirectUris= { "http://localhost:4200" },
                    //详见:IdentityServerConstants.StandardScopes
                        AllowedScopes={
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Phone,
                        "Drypoint_Host_API",
                        DrypointConsts.RolesScope
                    },
                    AlwaysIncludeUserClaimsInIdToken= true,
                    AllowOfflineAccess= true
                }
            };
        }

        public static List<ApplicationUser> GetTestUser()
        {
            List<ApplicationUser> Users = new List<ApplicationUser>();

            var user1 = new ApplicationUser
            {
                UserName = "admin",
                PasswordHash = "123456",
            };

            //var user1Claims =
            //        {
            //            new Claim(JwtClaimTypes.Name, "Alice Smith"),
            //            new Claim(JwtClaimTypes.GivenName, "Alice"),
            //            new Claim(JwtClaimTypes.FamilyName, "Smith"),
            //            new Claim(JwtClaimTypes.Email, "admin@email.com"),
            //            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
            //            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
            //            new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
            //            new Claim(JwtClaimTypes.Role, "admin")
            //        }

            var user2 = new ApplicationUser
            {
                UserName = "user",
                PasswordHash = "123456",
            };
            //var user2Claims =
            //    {
            //        new Claim(JwtClaimTypes.Name, "Bob Smith"),
            //        new Claim(JwtClaimTypes.GivenName, "Bob"),
            //        new Claim(JwtClaimTypes.FamilyName, "Smith"),
            //        new Claim(JwtClaimTypes.Email, "user@email.com"),
            //        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
            //        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
            //        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
            //        new Claim("location", "somewhere"),
            //        new Claim(JwtClaimTypes.Role, "user")
            //    }
            //};

            Users.Add(user1);
            Users.Add(user2);

            return Users;
        }
    }
}
