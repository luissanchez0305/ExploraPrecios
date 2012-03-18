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

            var mailMsg = new MailMessage();
            mailMsg.To.Add(new MailAddress(ConfigurationManager.AppSettings.Get("emailTo")));
            mailMsg.From = new MailAddress(ConfigurationManager.AppSettings.Get("emailFrom")); //Cambiar por el que envia el usuario

            mailMsg.Subject = "ExploraPrecios Contact Entry";
            mailMsg.Body = @"Name: " + ContactVM.name + "<br/>" +
                            "Email: " + ContactVM.email + "<br/>" +
                            "Message: " + ContactVM.message;

            mailMsg.IsBodyHtml = true;

            NetworkCredential credential = new NetworkCredential(ConfigurationManager.AppSettings.Get("emailFrom"), ConfigurationManager.AppSettings.Get("emailPass"));
            var smtpClient = new System.Net.Mail.SmtpClient();
            smtpClient.Credentials = credential;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Host = ConfigurationManager.AppSettings.Get("emailHost");
            smtpClient.Port = 25;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Send(mailMsg);

            // Enviar otro correo al usuario que nos envio el mensaje de que se le va contestar en 24 horas.
            var mailMsgToUser = new MailMessage();
            mailMsgToUser.To.Add(new MailAddress(ContactVM.email));
            mailMsgToUser.From = new MailAddress(ConfigurationManager.AppSettings.Get("emailNoReply"), "ExploraPrecios AutoRespuesta");
            mailMsgToUser.Subject = "Gracias por contactarnos";
            mailMsgToUser.Body = @"Estamos trabajando en su consulta. En menos de 24 horas recibirá respuesta por parte de uno de nuestros representantes.<br/><br/>
                                Saludos,<br/>
                                Equipo de ExploraPrecios.com";
            mailMsgToUser.IsBodyHtml = true;

            var smtpClientToUser = new System.Net.Mail.SmtpClient();
            smtpClientToUser.Credentials = credential;
            smtpClientToUser.UseDefaultCredentials = false;
            smtpClientToUser.Host = ConfigurationManager.AppSettings.Get("emailHost");
            smtpClientToUser.Port = 25;
            smtpClientToUser.DeliveryMethod = SmtpDeliveryMethod.Network;
            //No se enviara el segundo email para el usuario ya que sale un error de relay
            //smtpClientToUser.Send(mailMsgToUser);
            ContactVM.success = true;
            return View("Index", ContactVM);
        }
    }
}
