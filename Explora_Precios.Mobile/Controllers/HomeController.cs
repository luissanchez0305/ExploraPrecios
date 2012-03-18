using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Explora_Precios.Mobile.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Pronto ExploraPrecios.com en tu mobile";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
