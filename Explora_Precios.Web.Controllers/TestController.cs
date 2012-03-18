using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Explora_Precios.ApplicationServices;
using Explora_Precios.Core.DataInterfaces;

namespace Explora_Precios.Web.Controllers
{
    public class TestController : Controller
    {
        IClientRepository _clientRepository;

        public TestController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public ActionResult Index()
        {
            //var _clientObj = new ClientServices();
            //ViewData["data"] = _clientObj.GetClientItems(_clientRepository.Get(1), 2, 1);
            return View();
        }
    }
}
