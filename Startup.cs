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
            // ע��������
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // ���沿��
            services.AddScoped<ICaching, MemoryCaching>();
            services.AddSingleton<IMemoryCache>(factory =>
            {
                var value = factory.GetRequiredService<IOptions<MemoryCacheOptions>>();
                var cache = new MemoryCache(value);
                return cache;
            });

            //ע��appsettings��ȡ��
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

            //ֱ��ע��ĳһ����ͽӿ�
            //��ߵ���ʵ���࣬�ұߵ�As�ǽӿ�
            builder.RegisterType<AdvertisementServices>().As<IAdvertisementServices>();

            // Autofac��ֻ�Խӿڷ��� ���� ��virtual����������д����override�������������á�
            builder.RegisterType<BlogLogAOP>();//����ֱ���滻������������һ��Ҫ������������ע��
            builder.RegisterType<BlogCacheAOP>();

            //ע��Ҫͨ�����䴴�������
            var servicesDllFile = Path.Combine(basePath, "WebApi.Services.dll");
            var assemblysServices = Assembly.LoadFrom(servicesDllFile);

            builder.RegisterAssemblyTypes(assemblysServices)
                      .AsImplementedInterfaces()
                      .InstancePerLifetimeScope()
                      .EnableInterfaceInterceptors()
                      .InterceptedBy(typeof(BlogLogAOP), typeof(BlogCacheAOP));//���Է�һ��AOP����������
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
            // ��֤
            app.UseAuthentication();
            // ��Ȩ
            app.UseAuthorization();

            // ��·�м��
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
