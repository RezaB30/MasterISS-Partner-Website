using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class SMSConfirmationViewModel
    {
        public bool SmsIsSuccess { get; set; }
        public long CustomerId { get; set; }
        public string ConfirmationCode { get; set; }
    }
}