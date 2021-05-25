using MasterISS_Partner_WebSite_Business.Abstract;
using MasterISS_Partner_WebSite_Core.Utilities.Results;
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

        public IResult Add(User user)
        {
            if (user.NameSurname.Length < 2)
            {
                return new ErrorResult("En az 2 karakter Yap");
            }
            _userdal.Add(user);
            return new SuccessResult();
        }

        public IDataResult<List<User>> GetAll()
        {
            if (DateTime.Now.Hour == 14)
            {
                return new ErrorDataResult<List<User>>("hata var");
            }
            return new SuccessDataResult<List<User>>(_userdal.GetAll(), "Başarılı");
        }

        public IDataResult<User> GetById(long userId)
        {
            if (DateTime.Now.Hour == 1)
            {
                return new ErrorDataResult<User>("hata var");
            }
            return new SuccessDataResult<User>(_userdal.Get(u => u.Id == userId), "Başarılı");
        }
    }
}
