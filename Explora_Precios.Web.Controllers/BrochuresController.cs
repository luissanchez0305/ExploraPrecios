using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Explora_Precios.Web.Controllers
{
    public class BrochuresController : PrimaryController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
