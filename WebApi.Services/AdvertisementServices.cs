using System;
using System.Collections.Generic;
using WebApi.IServices;
using WebApi.Model;

namespace WebApi.Services
{
    public class AdvertisementServices : IAdvertisementServices
    {
        public int Test()
        {
            return 1;
        }

        public List<AdvertisementEntity> TestAOP() => new List<AdvertisementEntity>() { new AdvertisementEntity() { id = 1, name = "linzongdaiwoxue " } };
    }
}
