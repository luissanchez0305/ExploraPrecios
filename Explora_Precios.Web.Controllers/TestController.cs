using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Explora_Precios.ApplicationServices;
using Explora_Precios.Core.DataInterfaces;

namespace Explora_Precios.Web.Controllers
{
	public class TestController : PrimaryController
	{
		IClientRepository _clientRepository;

		public TestController(IClientRepository clientRepository)
		{
			_clientRepository = clientRepository;
		}

		public ActionResult Index(string postValue = "")
		{
			if (postValue != null)
			{
				var FBclient = new Facebook.FacebookClient();
				FBclient.AccessToken = CurrentUser.facebookToken;
				var parameters = new Dictionary<string, object>
				{
					{"access_token",  CurrentUser.facebookToken},
					{"appId", "285146028212857"},
					{"message", postValue},
					{"caption", "This is the caption" },
					{"description", "This is the description" },
					{"name", "This is the product name"},
					{"picture", "http://images.appleinsider.com/product-red-ipod-mock2.gif"},
					{"link", string.Format("http://www.exploraprecios.com?i={0}", 101)}
				};

				try
				{
					var result = FBclient.Post("72905468242/feed", parameters);
				}
				catch (Exception ex)
				{
					ViewData["result"] = "ERROR " + ex.Message;
					return View();
				}
				ViewData["result"] = "KUDOS!!";
			}
			return View();
		}
	}
}
