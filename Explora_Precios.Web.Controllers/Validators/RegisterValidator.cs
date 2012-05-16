using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explora_Precios.Web.Controllers.Validators
{
    public class RegisterValidator
    {
		public IEnumerable<KeyValuePair<string, string>> Validate(string Email)
		{
			if (string.IsNullOrEmpty(Email))
				yield return new KeyValuePair<string, string>("email", "Por favor introduzca un correo electronico");
		}

        public IEnumerable<KeyValuePair<string, string>> Validate(string Email, string Name, string LastName, DateTime Birthdate)
        {
            if (string.IsNullOrEmpty(Email))
                yield return new KeyValuePair<string, string>("email", "Por favor introduzca un correo electronico");
            if (string.IsNullOrEmpty(Name))
                yield return new KeyValuePair<string, string>("name", "Por favor introduzca un nombre");
            if (string.IsNullOrEmpty(LastName))
                yield return new KeyValuePair<string, string>("lastName", "Por favor introduzca el apellido");
            if (Birthdate.Year == 1)
                yield return new KeyValuePair<string, string>("birthdate", "Por favor introduzca una fecha de nacimiento valida");
        }
    }
}
