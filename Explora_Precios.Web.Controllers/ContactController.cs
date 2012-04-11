using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Net.Mail;
using System.Configuration;
using Explora_Precios.Web.Controllers.ViewModels;
using Explora_Precios.Web.Controllers.Validators;
using System.Net;
using Explora_Precios.ApplicationServices;

namespace Explora_Precios.Web.Controllers
{
	public class ContactController : PrimaryController
	{
		public ActionResult Index()
		{
			var contactVM = new ContactViewModel();
			return View(contactVM);
		}

		[AcceptVerbs(HttpVerbs.Post)]        
		public ActionResult SendContact() {

			// Crear el modelo
			var ContactVM = new ContactViewModel();
			bool isValid = TryUpdateModel(ContactVM);
			// Crear la validacion del modelo
			var results = new ContactValidator().Validate(ContactVM);
			if (!isValid || !results.isValid())
			{
				ModelState.AddRuleViolations(results);
				return View("Index", ContactVM);
			}

			var selfMessageBody = @"Name: " + ContactVM.name + "<br/>" +
							"Email: " + ContactVM.email + "<br/>" +
							"Message: " + ContactVM.message;

			var selfMessageEmail = new EmailServices(
				ConfigurationManager.AppSettings.Get("emailTo"),
				"ExploraPrecios Contact Entry",
				selfMessageBody);
			selfMessageEmail.Send();

			var usersMessageBody = @"Estamos trabajando en su consulta. En menos de 24 horas recibirá respuesta por parte de uno de nuestros representantes.<br/><br/>
								Saludos,<br/>
								Equipo de ExploraPrecios.com";
			var usersMessageEmail = new EmailServices(
				ContactVM.email,
				"Gracias por contactarnos",
				usersMessageBody);
			usersMessageEmail.Send();

			ContactVM.success = true;

			return View("Index", ContactVM);
		}
	}
}
