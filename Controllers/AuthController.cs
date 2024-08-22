using AddToCart.Context;
using AddToCart.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace AddToCart.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext db;
        public AuthController(ApplicationDbContext db)
        {
            this.db = db;
        }
        [AcceptVerbs("Post","Get")]
        public IActionResult CheckExistingId(string email)
        {
            var data=db.Users.Where(x=>x.Email==email).SingleOrDefault();
            if (data != null)
            {
                return Json($"Email {email} Already Exists");
            }
            else { 
            
            return Json(true);
            }
        }

        public static string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return null;
            }
            else
            {
                byte[] pass=ASCIIEncoding.ASCII.GetBytes(password);
                string encrpass =Convert.ToBase64String(pass);
                return encrpass;
            }
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(User u)
        {
            u.role = "User";
            u.Password=EncryptPassword(u.Password);
            db.Users.Add(u);
            db.SaveChanges();

            return RedirectToAction("Login");
        }
        
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(Login log)
        {
            var data = db.Users.Where(x => x.Email == log.email).SingleOrDefault();
            if (data != null)
            {
                log.password = EncryptPassword(log.password);
                if (log.password == data.Password)
                {
                    HttpContext.Session.SetString("MyUser", data.Email);
                    return RedirectToAction("Index","Dashboard");
                }
                else 
                {
                    TempData["ErrorPassword"] = "Invalid Password";
                }
            }
            else 
            {
                TempData["ErrorEmail"] = "Invalid Email";
            }
            return View();
        }

        public IActionResult AllUser()
        {
            var data =db.Users.ToList();
            return Json(data);
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}
