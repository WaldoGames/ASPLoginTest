using Core.Interfaces;
using Core.Models.DTO;
using Dal.Classes;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using System.Text;
using View.Models;
namespace View.Controllers
{
    public class LoginController : Controller
    {
        ILoginControler lc = new ImplementedLoginController();

        public ActionResult Index()
        {
            if(GetUserFromSession(out UserDto dto))
            {
                return RedirectToAction("LogedIn");
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginObject LoginObject)
        {

            if(!lc.Login(LoginObject.username, LoginObject.password, out UserDto user))
            {
                return View();
            }

            HttpContext.Session.SetString("Token", user.rememberToken);
            HttpContext.Session.SetString("Username", user.username);
            HttpContext.Session.SetString("Email", user.email);
            HttpContext.Session.SetString("Id", user.id.ToString());

            return RedirectToAction("LogedIn");
        }
        public ActionResult LogedIn()
        {
            UserObject user = new UserObject();
            user.username=HttpContext.Session.Get("Username").ToString();
            user.email = HttpContext.Session.Get("Email").ToString();
            return View(user);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateNewUser nu)
        {


            NewUserDto newUserDto = new NewUserDto();
            newUserDto.username = nu.username;
            newUserDto.email = nu.email;
            newUserDto.password = nu.password;

            lc.CreateNewUser(newUserDto);

            return RedirectToAction("Index");
        }

        public bool GetUserFromSession(out UserDto? user)
        {
            user = new UserDto();
            try
            {
                
                HttpContext.Session.TryGetValue("Token", out byte[] b);
                user.rememberToken = Encoding.UTF8.GetString(b);
                HttpContext.Session.TryGetValue("Username", out byte[] b2);
                user.username = Encoding.UTF8.GetString(b2);
                HttpContext.Session.TryGetValue("Email", out byte[] b3);
                user.email = Encoding.UTF8.GetString(b3);
                HttpContext.Session.TryGetValue("Id", out byte[] b4);
                user.id = Convert.ToInt32(Encoding.UTF8.GetString(b4));

                return true;

            }
            catch (Exception)
            {
                return false;
                throw;
            }

            
        }

    }
}
