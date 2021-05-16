using MasterISS_Partner_WebSite_Core.DataAccess.EntityFramework;
using MasterISS_Partner_WebSite_DataAccess.Abstract;
using MasterISS_Partner_WebSite_Database.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MasterISS_Partner_WebSite_DataAccess.Concrete.EntityFramework
{
    public class EfUserDal : EfEntityRepositoryBase<User, PartnerWebSiteEntities>, IUserDal
    {
        //IUserDal lazım çünkü sadece user tablosuna özel kodlar yazmak istersek IUserDal interfacesine tanımlycaz.
    }
}
