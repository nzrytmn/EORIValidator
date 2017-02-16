using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace euro.web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var validator = new EuroValidation();
            var result =  validator.ValidateEoriNumber(new List<string> { "DE123456", "IT123456789" });
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}