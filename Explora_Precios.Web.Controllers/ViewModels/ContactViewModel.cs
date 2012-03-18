    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explora_Precios.Web.Controllers.ViewModels
{
    public class ContactViewModel
    {
        public ContactViewModel() { success = false; }

        public string email { get; set; }
        public string name { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public string captchaFirst { get; set; }
        public string captchaSecond { get; set; }
        public string captchaSign { get; set; }
        public string captchaValue { get; set; }
    }
}
