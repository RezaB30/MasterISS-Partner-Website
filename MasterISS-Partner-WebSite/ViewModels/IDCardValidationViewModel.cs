using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class IDCardValidationViewModel
    {
        public string BirtDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int IdCardType { get; set; }
        public string RegistirationNo { get; set; }
        public string TCKNo { get; set; }
    }
}