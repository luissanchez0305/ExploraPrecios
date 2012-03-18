using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Web.Controllers.ViewModels;
using System.Text.RegularExpressions;

namespace Explora_Precios.Web.Controllers.Validators
{
    public class ContactValidator
    {
        public IEnumerable<KeyValuePair<string, string>> Validate(ContactViewModel contactVM)
        {
            if (string.IsNullOrEmpty(contactVM.name))
                yield return new KeyValuePair<string, string>("name", "Por favor introduzca un nombre");
            if (string.IsNullOrEmpty(contactVM.message))
                yield return new KeyValuePair<string, string>("message", "Por favor introduzca un mensaje");
            string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
               + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
               + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
               + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
               + @"[a-zA-Z]{2,}))$";
            Regex reStrict = new Regex(patternStrict);
            if (string.IsNullOrEmpty(contactVM.email))
                yield return new KeyValuePair<string, string>("email", "Por favor introduzca un email");
            else
                if (!reStrict.IsMatch(contactVM.email))
                    yield return new KeyValuePair<string, string>("email", "Por favor introduzca un email valido");
            var captchValue = 0;
            if (string.IsNullOrEmpty(contactVM.captchaValue) || !int.TryParse(contactVM.captchaValue, out captchValue))
            {
                yield return new KeyValuePair<string, string>("captchaValue", "Respuesta invalida o incorrecta");
            }
            else
            {
                var result = new System.Data.DataTable().Compute(contactVM.captchaFirst + contactVM.captchaSign + contactVM.captchaSecond, null);
                if (Convert.ToInt32(result) != Convert.ToInt32(contactVM.captchaValue))
                    yield return new KeyValuePair<string, string>("captchaValue", "Respuesta invalida o incorrecta");
            }

        }

    }
}
