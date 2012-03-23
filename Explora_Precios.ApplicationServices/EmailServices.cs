using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Explora_Precios.ApplicationServices
{
    public class EmailServices
    {
        protected string ToEmail { get; set; } 
        protected string EmailSubject { get; set; }
        protected string EmailBody { get; set; }
        protected bool SendBCCEmail { get; set; }

        public EmailServices(string Email, string Subject, string Body)
        {
            ToEmail = Email;
            EmailSubject = Subject;
            EmailBody = Body;
            SendBCCEmail = false;
        }

        public EmailServices(string Email, string Subject, string Body, bool SendBCC)
        {
            ToEmail = Email;
            EmailSubject = Subject;
            EmailBody = Body;
            SendBCCEmail = SendBCC;
        }

        public bool Send()
        {
            string OurEmail = System.Configuration.ConfigurationManager.AppSettings["emailNoReply"];
            string NameOurEmail = System.Configuration.ConfigurationManager.AppSettings["nameNoReply"];
            var Info = new System.Net.NetworkCredential(OurEmail, System.Configuration.ConfigurationManager.AppSettings["emailPass"]);
            var Msg = new MailMessage();
            Msg.IsBodyHtml = true;
            Msg.To.Add(new MailAddress(ToEmail));
            if (SendBCCEmail)
                Msg.Bcc.Add(new MailAddress("lsanchez@exploraprecios.com"));
            Msg.From = new MailAddress(OurEmail, NameOurEmail);
            Msg.Priority = MailPriority.Normal;
            Msg.Subject = EmailSubject;
            Msg.Body = EmailBody;
            var Smtp = new SmtpClient(System.Configuration.ConfigurationManager.AppSettings["emailHost"]);
            Smtp.UseDefaultCredentials = false;
            Smtp.Credentials = Info;
            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            Smtp.Port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["emailPort"]);

            try
            {
                Smtp.Send(Msg);
                return true;
            }
            catch (Exception ex)
            { throw ex; }
        }
    }
}
