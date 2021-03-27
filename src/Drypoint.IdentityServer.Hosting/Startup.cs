using Drypoint.IdentityServer.Hosting.Authorization;
using Drypoint.IdentityServer.Hosting.Extensions;
using Drypoint.IdentityServer.Hosting.HostService;
using Drypoint.IdentityServer.Hosting.Models;
using Drypoint.IdentityServer.Hosting.ToolKit;
using Drypoint.IdentityServer.Hosting.ToolKit.EnumCollection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace Drypoint.IdentityServer.Hosting
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSameSiteCookiePolicy();

            //注册Http上下文访问服务
            services.AddHttpContextAccessor();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
        
            #region CORS
            services.AddCors(options =>
            {
                options.AddPolicy(DrypointConst.LocalCorsPolicyName, builder =>
                {
                    builder
                        //.WithOrigins(
                        //    // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                        //    Configuration["App:CorsOrigins"]
                        //        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        //        .Select(o => o.RemovePostFix("/"))
                        //        .ToArray()
                        //)
                        //.SetIsOriginAllowedToAllowWildcardSubdomains()
                        .SetIsOriginAllowed(ori => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            #endregion

            #region SQL
            var dbCategory = DBCategoryEnum.PostgreSQL;
            services.AddDrypointIdentityServerDbContext(Configuration, dbCategory);
            #endregion 

            #region https
            //注册Hsts（传输安全）服务
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                //options.ExcludedHosts.Add("example.com");
            });
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 443;
            });
            #endregion

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/oauth2/authorize");
            });

            //配置session的有效时间,单位秒
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(30);
            });

            #region MVC
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                //options.Filters.Add(new CorsAuthorizationFilterFactory(LocalCorsPolicyName));
            }).AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key,按照Model中的属性名进行命名
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";

            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            #endregion

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            #region 注册 IdentityServer4 服务
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.UserInteraction = new IdentityServer4.Configuration.UserInteractionOptions
                {
                    LoginUrl = "/oauth2/authorize",//登录地址  
                };
            })// 自定义验证，可以不走Identity
              //自定义用户密码验证模式
              //.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();
              .AddExtensionGrantValidator<WeiXinOpenGrantValidator>()

                // 数据库模式
                .AddAspNetIdentity<ApplicationUser>()

                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore()
                .AddOperationalStore(options =>
                {
                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    // options.TokenCleanupInterval = 15; // frequency in seconds to cleanup stale grants. 15 is useful during debugging
                });

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            #endregion

            #region BackgroundService
            services.AddHostedService<InitDataService>();
            #endregion
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.Requirements.Add(new ClaimRequirement("rolename", "Admin")));
                options.AddPolicy("SuperAdmin", policy => policy.Requirements.Add(new ClaimRequirement("rolename", "SuperAdmin")));
            });

            services.AddSingleton<IAuthorizationHandler, ClaimsRequirementHandler>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                app.UseHsts();

                //开启HTTPS重定向
                app.UseHttpsRedirection();
            }

            app.UseCookiePolicy();
            app.UseSession();
            //注册静态文件到管道
            app.UseStaticFiles();
            //压缩 由于UseStaticFiles在之前 故不压缩静态文件
            //app.UseResponseCompression();
            //app.UseCookiePolicy();
            //app.UseRouting();
            //注册路由到管道
            app.UseRouting();

            //注册跨域策略到管道
            app.UseCors(DrypointConst.LocalCorsPolicyName);

            //授权相关：服务端代码  注册IdentityServer4到管道
            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
