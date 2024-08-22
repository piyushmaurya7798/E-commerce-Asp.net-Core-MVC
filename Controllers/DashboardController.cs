using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AddToCart.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
                if (HttpContext.Session.GetString("MyUser") == null)
                {
                    return RedirectToAction("Login", "Auth");
            }
            else 
            {
            string suser=HttpContext.Session.GetString("MyUser");
                TempData["suser"]=suser;
            return View();
            }
        }
        public IActionResult About()
        {
        
            return View();
        }
       
    }
}
