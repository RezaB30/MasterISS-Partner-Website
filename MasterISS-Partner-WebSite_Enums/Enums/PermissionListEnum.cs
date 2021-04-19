using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite_Enums
{
    public enum PermissionListEnum
    {
        SaleManager = 1,
        SetupManager = 2,
        PaymentManager = 3,
        UpdateTaskStatus = 10,
        PaymentCreditReportDetail = 11,
        PaymentCreditReportNotDetail = 14,
        SetupRevenuesList = 16,
        SaleRevenuesList = 17,
        RendezvousTeam = 18,
        LastPayments = 19,
    }
}