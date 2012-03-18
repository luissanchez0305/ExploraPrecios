using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Net.Mail;
using System.Net;
using Explora_Precios.Core.DataInterfaces;
using System.IO;
using Explora_Precios.ApplicationServices;

namespace Explora_Precios.Web.Controllers.Helpers
{
    public static class AccountHelper
    {
        public static string NextRandom(int size)
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * r.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public enum EmailType { ForgotPassword, ChangeEmail, NewUser}
        
        public static bool SendEmail(EmailType Type, string Name, string Email)
        {
            var Code = "";
            return SendEmail(Type, Email, Name, out Code);
        }
        public static bool SendEmail(EmailType Type, string Email, string Name, out string Code)
        {
            var MembershipUser = Membership.GetUser(Email);
            var Body = "";
            var Subject = "";
            var FilePath = "";
            var Html = "";
            var Address = "";
            Code = "";
            switch (Type)
            {
                case EmailType.ForgotPassword:
                    var TempPassword = MembershipUser.ResetPassword();
                    var NewPassword = NextRandom(8);
                    MembershipUser.ChangePassword(TempPassword, NewPassword);
                    Subject = "ExploraPrecios.com - Contraseña Reinicializada";
                    FilePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "").Replace("/bin", "")) + "\\Content\\html\\forgot.html";
                    Html = File.ReadAllText(FilePath);
                    Body = Html.Replace("<name>", Name).Replace("<email>", Email).Replace("<pwd>", NewPassword);
                    break;
                case EmailType.NewUser:
                    Code = NextRandom(10);
                    Subject = "ExploraPrecios.com - Usuario Creado";
                    FilePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "").Replace("/bin", "")) + "\\Content\\html\\welcome.html";
                    Html = File.ReadAllText(FilePath);
                    Address = "http://www.exploraprecios.com/Account/Valid?code="+Code+"&email="+Email;
                    Body = Html.Replace("<email>", MembershipUser.Email).Replace("<code>", Code).Replace("<address>", Address).Replace("<name>", Name);
                    break;
                case EmailType.ChangeEmail:
                    Code = NextRandom(10);
                    Subject = "ExploraPrecios.com - Correo Actualizado";
                    FilePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "").Replace("/bin", "")) + "\\Content\\html\\changeemail.html";
                    Html = File.ReadAllText(FilePath);
                    Address = "http://www.exploraprecios.com/Account/Valid?code="+Code+"&email="+Email;
                    Body = Html.Replace("<name>", Name).Replace("<email>", Email).Replace("<code>", Code).Replace("<address>", Address);             
                    break;
                default:
                    throw new Exception("What's up??");
            }

            var EmailService = new EmailServices(Email, Subject, Body);

            return EmailService.Send();
        }

    }
}
