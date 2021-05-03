using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class SaveCustomerAttachmentViewModel
    {
        [Display(ResourceType = typeof(Localization.Model), Name = "AttachmentType")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public int AttachmentType { get; set; }
        public byte[] FileContect { get; set; }
        public string FileExtention { get; set; }

        public long SubscriptionId { get; set; }
    }
}