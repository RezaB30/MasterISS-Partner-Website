using MasterISS_Partner_WebSite_Core.DataAccess;
using MasterISS_Partner_WebSite_Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterISS_Partner_WebSite_DataAccess.Abstract
{
    public interface IUserDal : IEntityRepository<User>
    {
    }
}
