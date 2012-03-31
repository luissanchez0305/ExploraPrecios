using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Explora_Precios.ApplicationServices;

namespace Explora_Precios.Web.Controllers
{
	public class ErrorController : PrimaryController
	{
		public ActionResult Index()
		{
			var email = new EmailServices("info@exploraprecios.com", "Error", "Error YA! " + CurrentUser != null ? CurrentUser.username : "No user");
			email.Send();
			return View();
		}
	}
}
