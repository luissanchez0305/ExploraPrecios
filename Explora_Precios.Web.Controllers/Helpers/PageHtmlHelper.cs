using System;
using System.Text;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;

namespace Explora_Precios.Web.Controllers.Helpers
{
	public static class PageHtmlHelper
	{
		public static string GroupCreated(this TimeSpan diff)
		{
			if (diff.TotalDays > 365)
				return "Creado hace mas de 1 año";
			else if (diff.TotalDays > 31)
				return "Creado hace " + ((int)(diff.TotalDays / 31)).ToString() + " mes";
			else if (diff.TotalDays > 1 && diff.TotalDays < 2)
				return "Creado hace 1 dia";
			else if (diff.TotalDays > 1)
				return "Creado hace " + (diff.Days).ToString() + " dias";
			else if (diff.TotalHours > 1 && diff.TotalHours < 2)
				return "Creado hace " + diff.Hours.ToString() + " hora";
			else if (diff.TotalHours > 1)
				return "Creado hace " + diff.Hours.ToString() + " horas";
			else if (diff.TotalMinutes > 1)
				return "Creado hace " + diff.Minutes.ToString() + " minutos";
			else
				return "Recien creado";
		}

		public static string ToArrayString(this List<int> list)
		{
			var arrayString = "";

			foreach (int val in list)
			{
				arrayString += val.ToString() + ",";
			}

			return arrayString.Substring(0, arrayString.Length - 1);    
		}
		public static string Captcha(this HtmlHelper helper) {
			
			var randomSign = new Random().Next(1);
			var randomNumbers = new Random();
			var firstNumber =randomNumbers.Next(10);
			var secondNumber = randomNumbers.Next(randomSign == 0 ? 10 : firstNumber);

			var sb = new StringBuilder();

			sb.Append(string.Format("<table class=\"captcha\"><tr><td>Por favor, conteste lo siguiente: {0} " + (randomSign == 0 ? "+" : "-") + " {1} = </td>", firstNumber, secondNumber));
			sb.Append("<td><input type=\"text\" name=\"captchaValue\" id=\"captchaValue\" class=\"captchaval\"/> (requerido)");
			sb.Append("<input type=\"hidden\" name=\"captchaFirst\" id=\"captchaFirst\" value=\"" + firstNumber + "\"/>");
			sb.Append("<input type=\"hidden\" name=\"captchaSecond\"  id=\"captchaSecond\" value=\"" + secondNumber + "\"/>");
			sb.Append("<input type=\"hidden\" name=\"captchaSign\"  id=\"captchaSign\" value=\"" + (randomSign == 0 ? "+" : "-") + "\"/></td></tr></table>");
			return sb.ToString();
		}
		public static string Money(this float val)
		{
			return val.ToString("$#,0.00");
		}
	}

}
