using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApi.Common.Helper;
using WebApi.Common.MemoryCache;
using WebApi.Extention;
using WebApi.Extention.Authorizations;
using WebApi.Extention.Middleware;
using WebApi.Extention.ServiceExtensions;
using WebApi.IServices;
using WebApi.Services;
using WebApiDemo.AOP;
using static WebApi.Extention.Authorizations.JwtHelper;

namespace WebApiDemo
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            // 注入配置项
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 缓存部分
            services.AddScoped<ICaching, MemoryCaching>();
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var value = factory.GetRequiredService<IOptions<MemoryCacheOptions>>();
                var cache = new MemoryCache(value);
                return cache;
            });

            //注册appsettings读取类
            services.AddSingleton(new Appsettings(Configuration));

            // Swagger
            services.AddSwaggerSetup();

            services.AddAuthorizationSetup();
            services.AddAuthentication_JWTSetup();

            services.AddControllers();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var basePath = ApplicationEnvironment.ApplicationBasePath;

            //直接注册某一个类和接口
            //左边的是实现类，右边的As是接口
            builder.RegisterType<AdvertisementServices>().As<IAdvertisementServices>();

            // Autofac它只对接口方法 或者 虚virtual方法或者重写方法override才能起拦截作用。
            builder.RegisterType<BlogLogAOP>();//可以直接替换其他拦截器！一定要把拦截器进行注册
            builder.RegisterType<BlogCacheAOP>();

            //注册要通过反射创建的组件
            var servicesDllFile = Path.Combine(basePath, "WebApi.Services.dll");
            var assemblysServices = Assembly.LoadFrom(servicesDllFile);

            builder.RegisterAssemblyTypes(assemblysServices)
                      .AsImplementedInterfaces()
                      .InstancePerLifetimeScope()
                      .EnableInterfaceInterceptors()
                      .InterceptedBy(typeof(BlogLogAOP), typeof(BlogCacheAOP));//可以放一个AOP拦截器集合
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerMiddle();

            app.UseRouting();
            // 认证
            app.UseAuthentication();
            // 授权
            app.UseAuthorization();

            // 短路中间件
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
