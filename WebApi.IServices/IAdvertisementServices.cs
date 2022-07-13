using System;
using System.Collections.Generic;
using WebApi.Model;

namespace WebApi.IServices
{
    public interface IAdvertisementServices
    {
        int Test();
        List<AdvertisementEntity> TestAOP();
    }
}
