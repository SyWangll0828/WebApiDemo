using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace WebApi.Extention.Authorizations
{
    // 用抽象类实现
    public class MyRequirementHandler : AuthorizationHandler<MyRequirement>
    {
        //public Task HandleAsync(AuthorizationHandlerContext context)
        //{
        //    var requirement = context.Requirements.FirstOrDefault();

        //    return Task.CompletedTask;
        //}

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MyRequirement requirement)
        {
            // 这边的requirement及为自定义策略中实例化的集合
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
