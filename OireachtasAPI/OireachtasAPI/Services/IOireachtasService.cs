using System;
using System.Collections.Generic;

namespace OireachtasAPI.Services
{
    public interface IOireachtasService
    {
        List<dynamic> FilterBillsByLastUpdated(DateTime since, DateTime until);
        List<dynamic> FilterBillsSponsoredBy(string pId);
    }
}