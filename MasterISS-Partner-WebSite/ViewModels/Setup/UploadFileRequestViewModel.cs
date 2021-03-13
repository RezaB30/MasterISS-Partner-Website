using MasterISS_Partner_WebSite_Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels.Setup
{
    public class UploadFileRequestViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public long TaskNo { get; set; }

        [Display(ResourceType = typeof(Localization.Model), Name = "AttachmentType")]
        [Required(ErrorMessageResourceType = typeof(Localization.Validation), ErrorMessageResourceName = "Required")]
        public AttachmentTypeEnum AttachmentTypesEnum { get; set; }
        public string FileData { get; set; }
        public string Extension { get; set; }
    }
}