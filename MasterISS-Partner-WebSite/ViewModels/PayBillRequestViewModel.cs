﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class PayBillRequestViewModel
    {
        public string SubMail { get; set; }

        public long[] BillIDs { get; set; }
    }
}