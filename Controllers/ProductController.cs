using AddToCart.Context;
using AddToCart.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace AddToCart.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext db;
        private IWebHostEnvironment env;
        public ProductController(ApplicationDbContext db,IWebHostEnvironment env) 
        {
            this.db = db;
            this.env = env;
        }
        //public IActionResult Index()
        //{
        //    var data=db.Products.ToList();
        //    return View(data);
        //}
        //[HttpPost]
        public IActionResult Index(string order)
        {
            var data=db.Products.ToList();
            if (order == "Low To High")
            {
                data=db.Products.OrderBy(x=>x.Price).ToList();
            }
            else if (order == "High To Low")
            {
               data=db.Products.OrderByDescending(x=>x.Price).ToList();

            }
            else if(order =="All")
            {
                data = db.Products.Take(5).ToList();
            }
            return View(data);
        }
        
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(ProductViewModel p)
        {
            var path = env.WebRootPath;
            var filepath = "/Content/Images/" + p.Image.FileName;
            var fullpath=Path.Combine(path+filepath);
            uploadFile(p.Image, fullpath);
            var obj = new Product()
            {
                Name = p.Name,
                Category = p.Category,
                Price = p.Price,
                Description = p.Description,
                Image = filepath
            };
            db.Products.Add(obj);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public void uploadFile(IFormFile picture, string fullpath)
        {
            FileStream f = new FileStream(fullpath, FileMode.Create);
            picture.CopyTo(f);
        }


        public IActionResult AddToCart(int id)
        {
            var suser = HttpContext.Session.GetString("MyUser");
            if (id!=0)
            {
                
            var data=db.Products.Find(id);
            var obj = new Cart()
            {
                Name = data.Name,
                Category = data.Category,
                Price = data.Price,
                Description = data.Description,
                Image = data.Image,
                suser = suser
            };
            db.Carts.Add(obj);
            db.SaveChanges();
            TempData["AddToCart"] = "Successfully Added to Cart ";
            }
            var totalprice = 0.0;
            var datat2 = db.Carts.Where(x => x.suser == suser).ToList();
            foreach (var datat in datat2)
            {
                 totalprice = totalprice + datat.Price;
            }
            TempData["totalprice"] = totalprice;
            return View(datat2);
        }
        
        public IActionResult deletecartitem(int id)
        {
            var data=db.Carts.Find(id);
            db.Carts.Remove(data); 
            db.SaveChanges();
            return RedirectToAction("AddToCart");
        }
    }
}
