using AddToCart.Context;
using AddToCart.Migrations;
using AddToCart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Razorpay.Api;
using System.Drawing;
using System.Net.Mail;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;

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
            var obj = new AddToCart.Models.Product()
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
            string totalprice2 = totalprice.ToString();
            HttpContext.Session.SetString("totalprice", totalprice2);
            //TempData["totalprice"] = totalprice;
            return View(datat2);
        }
        
        public IActionResult deletecartitem(int id)
        {
            var data=db.Carts.Find(id);
            db.Carts.Remove(data); 
            db.SaveChanges();
            return RedirectToAction("AddToCart");
        }
        
        public IActionResult paymentgateway()
        {
            var suser = HttpContext.Session.GetString("MyUser");
            var data2 = db.Carts.Where(x => x.suser == suser).ToList();
            foreach (var data in data2) 
            {
                var obj = new AddToCart.Models.Payment()
                {
                    Name = data.Name,
                    Category = data.Category,
                    Price = data.Price,
                    suser = data.suser,
                    Image = data.Image,
                    Description = data.Description,
                    Created = DateTime.Now
                };
                db.Payments.Add(obj);
            db.Carts.Remove(data); 
                db.SaveChanges();
            }
            

            return RedirectToAction("AddToCart");
        }


        public IActionResult InitiateOrder()
        {
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", Convert.ToDecimal(HttpContext.Session.GetString("totalprice"))*100 ); // this amount should be same as transaction amount
            input.Add("currency", "INR");
            input.Add("receipt", "12121");

            string KeyId = "rzp_test_4DrJJevYZkd0No";
            string KeySecret = "3oX1Dvxlpy2wB3BGnDPxGtH8";

            RazorpayClient client = new RazorpayClient(KeyId, KeySecret);

            Razorpay.Api.Order order = client.Order.Create(input);
            ViewBag.orderid = order["id"].ToString();
            ViewBag.Amount = input["amount"];
            ViewBag.KeyId = KeyId;
//            string razorpayScript = $@"
//var options={{
//        'key'': '', // Enter the Key ID generated from the Dashboard
//        'amount':  '{Convert.ToDecimal(HttpContext.Session.GetString("totalprice")) * 100}' , // Amount is in currency subunits. Default currency is INR. Hence, 50000 refers to 50000 paise
//        'currency': 'INR',
//        'name': 'Acme Corp',
//        'description': 'Buy Green Tea',
//        'order_id': orderId,
//        'image': 'https://example.com/your_logo',
//        'theme': {{
//                'color': '#3399cc'
//        }}
//        }};

//var rzp =new Razorpay(options);
//rzp.open();";

            //ClientScript.RegisterStartupScript(this.GetType(), "razorpayScript", razorpayScript, true);
            return View();
        }


        public IActionResult Payment(string razorpay_payment_id,string razorpay_order_id,string razorpay_signature) 
        {
            RazorpayClient client = new RazorpayClient("[YOUR_KEY_ID]", "[YOUR_KEY_SECRET]");

            Dictionary<string, string> attributes = new Dictionary<string, string>();

            attributes.Add("razorpay_payment_id", razorpay_payment_id);
            attributes.Add("razorpay_order_id", razorpay_order_id);
            attributes.Add("razorpay_signature", razorpay_signature);

            Utils.verifyPaymentSignature(attributes);
            return RedirectToAction("Index");
        }
        
        public IActionResult DonePayment() 
        {
            var suser = HttpContext.Session.GetString("MyUser");
            var data2 = db.Carts.Where(x => x.suser == suser).ToList();
            var myEmail = "piyushmaurya7798@gmail.com";
            var pass = "qbposjoyllyywcld";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(myEmail, pass)
            };
                StringBuilder invoice = new StringBuilder();
            invoice.AppendLine("<html>");
            invoice.AppendLine("<head>");
            invoice.AppendLine("<style>");
            invoice.AppendLine("table { width: 100%; border-collapse: collapse; }");
            invoice.AppendLine("table, th, td { border: 1px solid black; }");
            invoice.AppendLine("th, td { padding: 10px; text-align: left; }");
            invoice.AppendLine(".header { font-size: 24px; font-weight: bold; }");
            invoice.AppendLine(".footer { font-size: 12px; color: gray; }");
            invoice.AppendLine("</style>");
            invoice.AppendLine("</head>");
            invoice.AppendLine("<body>");
            invoice.AppendLine("<div style='text-align: center;'>");
            //invoice.AppendLine("<img src='' style='width: 200px;' />");
            invoice.AppendLine("</div>");
            invoice.AppendLine("<h1 class='header'>Invoice</h1>");
            invoice.AppendLine("<p>Date: " + DateTime.Now.ToString("dd/MM/yyyy") + "</p>");
            invoice.AppendLine($"<p>Invoice Number: 1111</p>");
            invoice.AppendLine($"<p>Customer Name: {suser}</p>");
            invoice.AppendLine($"<p>Customer Email: {suser}</p>");
            invoice.AppendLine("<table>");
            invoice.AppendLine("<tr><th>ItemID</th><th>Name</th><th>Image</th><th>Description</th><th>Price</th></tr>");
            foreach (var data in data2)
            {
                invoice.AppendLine($"<tr><td>{data.Id}</td><td>{data.Name}</td><td><img src='{data.Image}'/></td><td>{data.Description}</td><td>&#8377;{data.Price:F2}</td></tr>");
                var obj = new AddToCart.Models.Payment()
                {
                    Name = data.Name,
                    Category = data.Category,
                    Price = data.Price,
                    suser = data.suser,
                    Image = data.Image,
                    Description = data.Description,
                    Created = DateTime.Now
                };
                db.Payments.Add(obj);
                db.Carts.Remove(data);
                db.SaveChanges();
            }
            string total=HttpContext.Session.GetString("totalprice");
            invoice.AppendLine($"<tr><td colspan='4' style='text-align: right;'>Total</td><td>&#8377;{total}</td></tr>");
            invoice.AppendLine("</table>");
            invoice.AppendLine("<p>Payment Terms: Net 30 days</p>");
            invoice.AppendLine("<p class='footer'>Thank you for your business!</p>");
            invoice.AppendLine("</body>");
            invoice.AppendLine("</html>");
            
            var subject = "Purchase Details";
            var mailmessage = new MailMessage {
                 From=new MailAddress(myEmail),
                 Subject=subject,
                 Body= invoice.ToString(),
                 IsBodyHtml=true,
                 
                 };

            mailmessage.To.Add(suser);
          client.Send(mailmessage);
            
            return RedirectToAction("AddToCart");
        }

    }
}
