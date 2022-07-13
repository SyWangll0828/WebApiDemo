using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace WebApi.Extention.Authorizations
{
    public class MyRequirementHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var requirement = context.Requirements.FirstOrDefault();

            return Task.CompletedTask;
        }
    }
}
