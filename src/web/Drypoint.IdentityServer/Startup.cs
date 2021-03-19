using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Drypoint.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Drypoint.Core.IdentityServer;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using AutoMapper;
using System;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Autofac;
using Drypoint.Application.AutoMapper;
using Drypoint.Unity.EnumCollection;
using IdentityServer4.Configuration;

namespace Drypoint.IdentityServer
{
    public partial class Startup
    {
        private const string LocalCorsPolicyName = "localhost";

        public IConfiguration Configuration { get; }
        //IWebHostEnvironment继承了IHostEnvironment 添加两个关于Web根目录的属性
        private IHostEnvironment Environment { get; }
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();

            //注册Http上下文访问服务
            services.AddHttpContextAccessor();
            services.AddHttpClient();

            #region AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AutoMapperConfig>();
            }, AppDomain.CurrentDomain.GetAssemblies());
            #endregion
            //DI
            //services.AddServiceRegister();

            # region MVC
            services.AddControllersWithViews();
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

            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            #endregion

            #region CORS
            services.AddCors(options =>
            {
                options.AddPolicy(LocalCorsPolicyName, builder =>
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
            services.AddDbContextConfigurer(Configuration, DBCategoryEnum.PostgreSQL);
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

            #region 注册 IdentityServer4 服务
            //授权相关：服务端代码
            services.AddIdentityServer(options =>
            {
                //活动事件 允许配置是否应该将哪些事件提交给注册的事件接收器
                options.Events = new EventsOptions
                {
                    RaiseErrorEvents = true,
                    RaiseFailureEvents = true,
                    RaiseInformationEvents = true,
                    RaiseSuccessEvents = true
                };
                
                //设置认证
                options.Authentication = new AuthenticationOptions
                {
                    CheckSessionCookieName = "idr.Cookies",  //用于检查会话端点的cookie的名称
                    CookieLifetime = new TimeSpan(1, 0, 0), //身份验证Cookie生存期（仅在使用IdentityServer提供的Cookie处理程序时有效）
                    CookieSlidingExpiration = true, //指定cookie是否应该滑动（仅在使用IdentityServer提供的cookie处理程序时有效）
                    RequireAuthenticatedUserForSignOutMessage = true //指示是否必须对用户进行身份验证才能接受参数以结束会话端点。默认为false
                };
                //用户交互页面定向设置处理
                options.UserInteraction = new UserInteractionOptions
                {
                    LoginUrl = "/Account/Login",    //【必备】登录地址  会覆盖全局未授权跳转地址替换掉aspnet Identity内置登录页，a标签与302跳转不受影响
                    LogoutUrl = "/Account/Logout",  //【必备】退出地址 
                    ConsentUrl = "/Account/Consent",    //【必备】允许授权同意页面地址
                    //ErrorUrl = "/Account/Error",      //【必备】错误页面地址
                    LoginReturnUrlParameter = "ReturnUrl", //【必备】设置传递给登录页面的返回URL参数的名称。默认为returnUrl 
                    LogoutIdParameter = "logoutId", //【必备】设置传递给注销页面的注销消息ID参数的名称。缺省为logoutId 
                    ConsentReturnUrlParameter = "ReturnUrl", //【必备】设置传递给同意页面的返回URL参数的名称。默认为returnUrl
                    ErrorIdParameter = "errorId", //【必备】设置传递给错误页面的错误消息ID参数的名称。缺省为errorId
                    CustomRedirectReturnUrlParameter = "ReturnUrl", //【必备】设置从授权端点传递给自定义重定向的返回URL参数的名称。默认为returnUrl
                    CookieMessageThreshold = 5 //【必备】由于浏览器对Cookie的大小有限制，设置Cookies数量的限制，有效的保证了浏览器打开多个选项卡，一旦超出了Cookies限制就会清除以前的Cookies值
                };
                // 缓存参数处理
                options.Caching = new CachingOptions
                {
                    ClientStoreExpiration = new TimeSpan(1, 0, 0), //设置Client客户端存储加载的客户端配置的数据缓存的有效时间 
                    ResourceStoreExpiration = new TimeSpan(1, 0, 0), // 设置从资源存储加载的身份和API资源配置的缓存持续时间
                    CorsExpiration = new TimeSpan(1, 0, 0) //设置从资源存储的跨域请求数据的缓存时间
                };

                options.Cors.PreflightCacheDuration = new TimeSpan(1, 0, 0);
            })
              //使用演示签名证书
              .AddDeveloperSigningCredential()
              //.AddSigningCredential(new X509Certificate2(Path.Combine(environment.ContentRootPath;, Configuration["Certs:Path"]), Configuration["Certs:Pwd"]))
              .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
              .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
              .AddInMemoryClients(IdentityServerConfig.GetClients())
              //使用内存测试数据身份认证 TODO
              .AddTestUsers(IdentityServerConfig.GetTestUser());
            //添加自定义claim
            //.AddProfileService<ProfileService>()
            //自定义用户密码验证模式
            //.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();

            /*
            services.AddAuthentication()
                //开启google登陆
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    //register your IdentityServer with Google at https://console.developers.google.com
                    //enable the Google + API
                    //set the redirect URI to http://localhost:5000/signin-google
                    options.ClientId = "xxxxxxxxxxxxxxxxxm";
                    options.ClientSecret = "xxxxxxxxxxxxxxxxx";
                });
            */
            #endregion
        }
        /// <summary>
        /// Autofac执行注入的地方 ConfigureServices之后执行
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacDIExtensionsModule());
        }

        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            logger.LogInformation("Begin Startup Configure......");

            //注册响应压缩到管道
            app.UseResponseCompression();

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "text/html";
                        var ex = context.Features.Get<IExceptionHandlerFeature>();
                        if (ex != null)
                        {
                            var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.StackTrace}";
                            context.Response.Headers.Add("application-error", ex.Error.Message);
                            context.Response.Headers.Add("access-control-expose-headers", "application-error");
                            context.Response.Headers.Add("access-control-allow-origin", "*");
                            await context.Response.WriteAsync(err).ConfigureAwait(false);
                        }
                    });
                });// this will add the global exception handle for production evironment.
                app.UseHsts();
                //开启HTTPS重定向
                app.UseHttpsRedirection();
            }


            //注册静态文件到管道
            app.UseStaticFiles();

            //注册路由到管道
            app.UseRouting();

            //注册跨域策略到管道
            app.UseCors(LocalCorsPolicyName);

            //授权相关：服务端代码  注册IdentityServer4到管道
            app.UseIdentityServer();
            //新版IdentityServer4要自己调用；
            app.UseAuthorization();
            

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            //});
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
