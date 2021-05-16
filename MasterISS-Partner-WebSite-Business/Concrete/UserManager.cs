using MasterISS_Partner_WebSite_Business.Abstract;
using MasterISS_Partner_WebSite_DataAccess.Abstract;
using MasterISS_Partner_WebSite_Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterISS_Partner_WebSite_Business.Concrete
{
    public class UserManager : IUserService
    {
        IUserDal _userdal;

        public UserManager(IUserDal userdal)
        {
            _userdal = userdal;
        }

        public List<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetById(long userId)
        {
            throw new NotImplementedException();
        }
    }
}
