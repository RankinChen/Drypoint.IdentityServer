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

            //ע��Http�����ķ��ʷ���
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
            //ע��Hsts�����䰲ȫ������
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

            //����session����Чʱ��,��λ��
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
                //����ѭ������
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //��ʹ���շ���ʽ��key,����Model�е���������������
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";

            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            #endregion

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            #region ע�� IdentityServer4 ����
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.UserInteraction = new IdentityServer4.Configuration.UserInteractionOptions
                {
                    LoginUrl = "/oauth2/authorize",//��¼��ַ  
                };
            })// �Զ�����֤�����Բ���Identity
              //�Զ����û�������֤ģʽ
              //.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();
              .AddExtensionGrantValidator<WeiXinOpenGrantValidator>()

                // ���ݿ�ģʽ
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

                //����HTTPS�ض���
                app.UseHttpsRedirection();
            }

            app.UseCookiePolicy();
            app.UseSession();
            //ע�ᾲ̬�ļ����ܵ�
            app.UseStaticFiles();
            //ѹ�� ����UseStaticFiles��֮ǰ �ʲ�ѹ����̬�ļ�
            //app.UseResponseCompression();
            //app.UseCookiePolicy();
            //app.UseRouting();
            //ע��·�ɵ��ܵ�
            app.UseRouting();

            //ע�������Ե��ܵ�
            app.UseCors(DrypointConst.LocalCorsPolicyName);

            //��Ȩ��أ�����˴���  ע��IdentityServer4���ܵ�
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
