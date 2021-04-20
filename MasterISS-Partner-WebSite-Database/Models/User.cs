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
    
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.PaidBillList = new HashSet<PaidBillList>();
            this.WorkArea = new HashSet<WorkArea>();
        }
    
        public long Id { get; set; }
        public Nullable<int> PartnerId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public string UserSubMail { get; set; }
        public string NameSurname { get; set; }
        public string Password { get; set; }
        public bool IsEnabled { get; set; }
        public string PhoneNumber { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaidBillList> PaidBillList { get; set; }
        public virtual RendezvousTeam RendezvousTeam { get; set; }
        public virtual Role Role { get; set; }
        public virtual SetupTeam SetupTeam { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkArea> WorkArea { get; set; }
    }
}
