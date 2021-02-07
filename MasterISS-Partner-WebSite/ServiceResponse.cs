using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }

        public string ErrorMessage { get; set; }
    }
}