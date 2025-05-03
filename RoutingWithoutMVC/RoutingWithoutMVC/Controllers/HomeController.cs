using Microsoft.AspNetCore.Mvc;

namespace RoutingWithoutMVC.Controllers
{
    //[Route("Home")]
    //Token
    [Route("[controller]")]
    [Route("[controller]/[action]")]

    public class HomeController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        //public IActionResult About()
        //{
        //    return View();
        //}


        //public int Details(int id)
        //{
        //    return id;
        //}


        //This is a arrtibute routing
        //[Route("")]
        //[Route("Index")]
        //[Route("~/")]
        //public IActionResult Index()
        //{
        //    return View();
        //}


        [Route("")]
        //[Route("[action]")]
        [Route("~/")]
        [Route("~/Home")]
        public IActionResult Index()
        {
            return View();
        }

        //Another approach
        //[Route("")]
        //[Route("Home")]
        //[Route("Home/Index")]
        //public IActionResult Data()
        //{
        //    return View("~/Views/Home/Index.cshtml");
        //}


        //[Route("About")]
        //public IActionResult About()
        //{
        //    return View();
        //}

        //[Route("[action]")]
        public IActionResult About()
        {
            return View();
        }

        //[Route("Details/{id?}")]
        //[Route("[action]/{id?}")]
        [Route("{id?}")]
        public int Details(int? id)
        {
            return id ?? 1;
        }

    }
}
