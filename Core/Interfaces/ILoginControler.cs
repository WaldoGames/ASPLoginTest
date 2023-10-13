using Core.Enums;
using Core.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ILoginControler
    {
        bool Login(string Username, string Password, out UserDto user);

        UserCreationEnum CreateNewUser(NewUserDto newUser);

        //bool Logout(UserDto user);

        bool ChecKLoginStatus(UserDto user);
    }
}
