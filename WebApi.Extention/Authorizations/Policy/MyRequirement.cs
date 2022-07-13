using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace WebApi.Extention.Authorizations
{
   public class MyRequirement : IAuthorizationRequirement
    {
        public string Name { get; set; }
    }
}
