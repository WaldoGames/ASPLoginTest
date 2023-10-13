using Core.Enums;
using Core.Interfaces;
using Core.Models.DTO;
using Dal.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Classes
{
    public class ImplementedLoginController : ILoginControler
    {
        DBContext dbContext = new DBContext();

        public bool ChecKLoginStatus(UserDto user)
        {
            return dbContext.IsTokenValid(user.id, user.rememberToken);
        }

        public UserCreationEnum CreateNewUser(NewUserDto newUser)
        {
            UserCreationEnum userCreationEnum = new UserCreationEnum();

            dbContext.TryAddUser(newUser,out userCreationEnum);

            return userCreationEnum;
        }

        public bool Login(string Username, string Password, out UserDto user)
        {
            if(dbContext.TryLogin(Username, Password, out user))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /*public bool Logout(UserDto user)
         {

         }*/
    }
}
