//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MasterISS_Partner_WebSite_Database.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UpdatedSetupStatus
    {
        public long Id { get; set; }
        public long TaskNo { get; set; }
        public Nullable<int> UserId { get; set; }
        public short FaultCodes { get; set; }
        public string Description { get; set; }
        public string ReservationDate { get; set; }
        public System.DateTime ChangeTime { get; set; }
    
        public virtual TaskList TaskList { get; set; }
    }
}
