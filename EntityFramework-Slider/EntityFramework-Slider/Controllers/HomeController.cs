using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;
using System.Diagnostics;

namespace EntityFramework_Slider.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            //HttpContext.Session.SetString("name", "Pervin");

            //Response.Cookies.Append("Surname", "Pervin",new CookieOptions { MaxAge = TimeSpan.FromMinutes(30)});


            //Book book = new Book
            //{
            //    Id= 1,
            //    Name = "Xosrov ve Shirin"
            //};

            //Response.Cookies.Append("book", JsonConvert.SerializeObject(book));

            List<Slider> sliders = await _context.Sliders.ToListAsync();

            SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync();

            IEnumerable<Blog> blogs = await _context.Blogs.Where(m=>!m.SoftDelete).ToListAsync();

            IEnumerable<Category> categories = await _context.Categories.Where(m => !m.SoftDelete).ToListAsync();

            IEnumerable<Product> products = await _context.Products.Include(m=>m.Images).Where(m => !m.SoftDelete).ToListAsync();



            int baketCount;

            if (Request.Cookies["basket"] != null)
            {
                baketCount = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]).Count;

            }
            else
            {
                baketCount = 0;
            }

            ViewBag.Count = baketCount;


           HomeVM model = new()
            {
                Sliders = sliders,
                SliderInfo = sliderInfo,
                Blogs = blogs,
                Categories = categories,
                Products = products
            };

            return View(model);



        }



        //Sessionda   tokeni goturub Json formatina kecirib Resturun edirik ve rootda /home/test/

        //public IActionResult Test()
        //{
        //    //Session'daki  datani goturur
        //    var sessionData = HttpContext.Session.GetString("name");


        //    var cookieData = Request.Cookies["Surname"];


        //    //var objectData =JsonConvert.DeserializeObject<Book>(Request.Cookies["book"]);

        //    //Token'ini Json vasitesi ile formatini deyisirik 
        //    return Json(objectData);


        //}


        [HttpPost]

        [ValidateAntiForgeryToken]
        public  async Task <IActionResult> AddBasket(int? id)
        {
            if (id is null) return BadRequest();

            Product dbProduct = await _context.Products.Include(m => m.Images).FirstOrDefaultAsync(m=>m.Id == id);

            if (dbProduct == null) return NotFound();


            List<BasketVM> basket;

            if (Request.Cookies["basket"] != null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
               
            }
            else
            {
                basket = new List<BasketVM>();
            }


            BasketVM? existProduct = basket?.FirstOrDefault(m => m.Id == dbProduct.Id);

           

            if (existProduct == null)
            {
                basket?.Add(new BasketVM
                {
                    Id = dbProduct.Id,
                    Count = 1,
                    Price = dbProduct.Price,
                    Image = dbProduct.Images.FirstOrDefault().Image,
                    Total = dbProduct.Price,
               




                });  

            }
            else
            {
                existProduct.Count++;
                existProduct.Total = (int)dbProduct.Price * existProduct.Count;

               

            }

           


            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));


            return RedirectToAction(nameof(Index));



        }

         
    }

    
   


    //class Book
    //{
    //    public int Id { get; set; }
    //    public string? Name { get; set; }
    //}





}