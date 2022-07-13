using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Extention.Authorizations;
using WebApi.IServices;
using WebApi.Model;

namespace WebApiDemo.Controllers
{
    /// <summary>
    /// 学生管理
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class StudentController : Controller
    {
        private readonly ILogger<StudentController> _logger;
        private readonly IAdvertisementServices _advertisementServices;

        public StudentController(ILogger<StudentController> logger, IAdvertisementServices advertisementServices)
        {
            _logger = logger;
            _advertisementServices = advertisementServices;
        }

        /// <summary>
        /// 获取所有学生信息
        /// </summary>
        [HttpPost]
        [Authorize]
        public void getAllStudentInfo(Student student)
        {

        }

        /// <summary>
        /// 测试AOP
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<AdvertisementEntity> TestAdsFromAOP()
        {
            return _advertisementServices.TestAOP();
        }

        [HttpGet]
        public string Login()
        {
            return JwtHelper.getJwtToken();
        }
    }
}
