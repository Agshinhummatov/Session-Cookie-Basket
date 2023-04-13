using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntityFramework_Slider.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {

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



            


            List<BasketVM> basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);

            return View(basket);
        }
    }
}
