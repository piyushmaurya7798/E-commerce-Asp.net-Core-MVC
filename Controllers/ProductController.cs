using AddToCart.Context;
using AddToCart.Models;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Index()
        {
            var data=db.Products.ToList();
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
            var filepath = "/Content/Images" + p.Image.FileName;
            var fullpath=Path.Combine(path,filepath);
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
    }
}
