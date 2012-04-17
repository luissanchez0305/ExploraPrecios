using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Explora_Precios.Data;
using SharpArch.Data.NHibernate;
using Explora_Precios.Core.DataInterfaces;
using Explora_Precios.Core;
using Facebook;
using System.Configuration;

namespace Explora_Precios.Web.Controllers
{
	public class PrimaryController : Controller
	{
		public User CurrentUser { get; set; }

		public PrimaryController()
		{
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewData["depList"] = new DepartmentRepository().GetAll();
			var Name = "";
			try
			{
				if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
				{
					CurrentUser = new UserRepository().GetByEmail(HttpContext.User.Identity.Name);
					ViewData["GroupSize"] = CurrentUser.groups.Count;
					if (!string.IsNullOrEmpty(CurrentUser.facebookToken))
					{
						var client = new FacebookClient();
						
						client.AccessToken = CurrentUser.facebookToken;
						try
						{
							var resultGet = (IDictionary<string, object>)client.Get("/me");
							Name = resultGet["first_name"].ToString();
						}
						catch
						{
							System.Web.Security.FormsAuthentication.SignOut();
						}
					}
					else
					{
						Name = CurrentUser.name;
					}
				}
			}
			catch
			{
			}

			ViewData["CurrentName"] = Name;
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			base.OnException(filterContext);

			var email = new Explora_Precios.ApplicationServices.EmailServices("info@exploraprecios.com",
				"Error en " + System.Configuration.ConfigurationManager.AppSettings["Enviroment"] + " - " + filterContext.Exception.Source,
				"Detalle: " + filterContext.Exception.StackTrace);
			email.Send();
		}

		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			//using (var tx = NHibernateSession.Current.BeginTransaction())
			//{

			//    SharpArch.Core.PersistenceSupport.IRepository<User> Update = new UserRepository();
			//    var Dictionary = new Dictionary<string, object>();
			//    Dictionary.Add("email", HttpContext.User.Identity.Name);
			//    ViewData["CurrentName"] = Update.FindAll(Dictionary)[0].name;

			//    tx.Commit();
			//}
			base.OnActionExecuted(filterContext);
		}
	}
}
