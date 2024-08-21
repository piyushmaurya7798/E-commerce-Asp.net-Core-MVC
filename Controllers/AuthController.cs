using AddToCart.Context;
using AddToCart.Models;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(User u)
        {
            u.role = "User";
            db.Users.Add(u);
            db.SaveChanges();

            return RedirectToAction("AllUser");
        }
        
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(User u)
        {
            return View();
        }

        public IActionResult AllUser()
        {
            var data =db.Users.ToList();
            return Json(data);
        }
    }
}
