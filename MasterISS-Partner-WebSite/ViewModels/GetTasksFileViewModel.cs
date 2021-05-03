using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite.ViewModels
{
    public class GetTasksFileViewModel
    {
        public List<GenericFileList> GenericFileList { get; set; }
    }

    public class GenericFileList
    {
        public string ImgLink{ get; set; }
        public string ImgSrc{ get; set; }
        public string AttachmentType { get; set; }
    }
}