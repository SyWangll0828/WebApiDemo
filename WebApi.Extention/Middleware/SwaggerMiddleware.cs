using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebApi.Common.Helper;
using static WebApi.Extention.ServiceExtensions.CustomApiVersion;

namespace WebApi.Extention.Middleware
{
    public static class SwaggerMiddleware
    {
        public static void UseSwaggerMiddle(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // 根据版本名称倒序 遍历展示
                var apiName = Appsettings.app(new string[] { "Startup", "ApiName" });
                typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                {
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{apiName} {version}");
                });

                c.SwaggerEndpoint($"https://petstore.swagger.io/v2/swagger.json", $"{apiName} pet");

                // todo
                // 将swagger首页，设置成我们自定义的页面，记得这个字符串的写法：{项目名.index.html}
                //if (streamHtml.Invoke() == null)
                //{
                //    var msg = "index.html的属性，必须设置为嵌入的资源";
                //    // Log.Error(msg);
                //    throw new Exception(msg);
                //}
                //c.IndexStream = streamHtml;

                //if (Permissions.IsUseIds4)
                //{
                //    c.OAuthClientId("blogadminjs");
                //}


                // 路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉，如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "doc";
                c.RoutePrefix = "doc";
            });
        }
    }
}
