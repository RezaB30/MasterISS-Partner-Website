using MasterISS_Partner_WebSite_Core.Utilities.Results;
using MasterISS_Partner_WebSite_Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterISS_Partner_WebSite_Business.Abstract
{
    public interface IUserService
    {
        IDataResult<List<User>> GetAll();

        IResult Add(User user);

        IDataResult<User> GetById(long userId);
    }
}
