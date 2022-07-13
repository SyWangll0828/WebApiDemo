﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using WebApi.Common.Helper;
using static WebApi.Extention.ServiceExtensions.CustomApiVersion;

namespace WebApi.Extention.ServiceExtensions
{
    public static class SwaggerSetup
    {
        public static void AddSwaggerSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var basePath = AppContext.BaseDirectory;
            //var basePath2 = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            var ApiName = Appsettings.app(new string[] { "Startup", "ApiName" });

            services.AddSwaggerGen(c =>
            {
                //遍历出全部的版本，做文档信息展示
                typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        Version = version,
                        Title = $"{ApiName} 接口文档——{RuntimeInformation.FrameworkDescription}",
                        Description = $"{ApiName} HTTP API " + version,
                        Contact = new OpenApiContact { Name = ApiName, Email = "619@xxx.com", Url = new Uri("https://neters.club") },
                        License = new OpenApiLicense { Name = ApiName + " 官方文档", Url = new Uri("http://www.baidu.com") }
                    });
                    c.OrderActionsBy(o => o.RelativePath);
                });

                c.UseInlineDefinitionsForEnums();
                try
                {
                    //这个就是刚刚配置的xml文件名
                    var xmlPath = Path.Combine(basePath, "WebApiDemo.xml");
                    //默认的第二个参数是false，这个是controller的注释，记得修改
                    c.IncludeXmlComments(xmlPath, true);

                    //这个就是Model层的xml文件名
                    var xmlModelPath = Path.Combine(basePath, "WebApi.Model.xml");
                    c.IncludeXmlComments(xmlModelPath);
                }
                catch (Exception ex)
                {
                    //log.Error("Blog.Core.xml和Blog.Core.Model.xml 丢失，请检查并拷贝。\n" + ex.Message);
                }

                // 开启加权小锁
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                // 在header中添加token，传递到后台
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                #region ids4和jwt切换
                //// ids4和jwt切换
                //if (Permissions.IsUseIds4)
                //{
                //    //接入identityserver4
                //    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                //    {
                //        Type = SecuritySchemeType.OAuth2,
                //        Flows = new OpenApiOAuthFlows
                //        {
                //            Implicit = new OpenApiOAuthFlow
                //            {
                //                AuthorizationUrl = new Uri($"{Appsettings.app(new string[] { "Startup", "IdentityServer4", "AuthorizationUrl" })}/connect/authorize"),
                //                Scopes = new Dictionary<string, string> {
                //                {
                //                    "blog.core.api","ApiResource id"
                //                }
                //            }
                //            }
                //        }
                //    });
                //}
                //else
                //{
                //    // Jwt Bearer 认证，必须是 oauth2
                //    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                //    {
                //        Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                //        Name = "Authorization",//jwt默认的参数名称
                //        In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                //        Type = SecuritySchemeType.ApiKey
                //    });
                //}
                #endregion

                // Jwt Bearer 认证，必须是 oauth2
                // Token绑定到ConfigureServices
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });

            });
        }
    }

    /// <summary>
    /// 自定义版本
    /// </summary>
    public class CustomApiVersion
    {
        /// <summary>
        /// Api接口版本 自定义
        /// </summary>
        public enum ApiVersions
        {
            /// <summary>
            /// V1 版本
            /// </summary>
            V1 = 1,
            /// <summary>
            /// V2 版本
            /// </summary>
            V2 = 2,
        }
    }
}
