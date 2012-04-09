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

		public static string Shorten(this string text, int lenght)
		{
			return text.Length >= lenght ? text.Substring(0, lenght) + "..." : text;
		}

		public static int CryptProductId(this string Id)
		{
			var productId = 0;
			if (!int.TryParse(Id, out productId))
			{
				productId = int.Parse(Id.DecryptString());
			}
			return productId;
		}

		public static string CryptProductId(this int Id)
		{
			return Id.ToString().EncryptString();			
		}

		public static string EncryptString(this string val)
		{
			var key = System.Configuration.ConfigurationManager.AppSettings["PublicKey"];
			var encryptedValue = IdEncrypter.EncryptStringAES(val, key);
			return encryptedValue;
		}

		public static string DecryptString(this string val)
		{
			var key = System.Configuration.ConfigurationManager.AppSettings["PublicKey"];
			var decryptedValue = IdEncrypter.DecryptStringAES(val, key);
			return decryptedValue;
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

	public static class IdEncrypter
	{
		private static byte[] _salt = Encoding.ASCII.GetBytes("o6806642kbM7c5");

		/// <summary>
		/// Encrypt the given string using AES.  The string can be decrypted using 
		/// DecryptStringAES().  The sharedSecret parameters must match.
		/// </summary>
		/// <param name="plainText">The text to encrypt.</param>
		/// <param name="sharedSecret">A password used to generate a key for encryption.</param>
		public static string EncryptStringAES(string value, string sharedSecret)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentNullException("value");
			if (string.IsNullOrEmpty(sharedSecret))
				throw new ArgumentNullException("sharedSecret");

			string outStr = null;                       // Encrypted string to return
			RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

			try
			{
				// generate the key from the shared secret and the salt
				Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

				// Create a RijndaelManaged object
				// with the specified key and IV.
				aesAlg = new RijndaelManaged();
				aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
				aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

				// Create a decrytor to perform the stream transform.
				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for encryption.
				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{

							//Write all data to the stream.
							swEncrypt.Write(value);
						}
					}
					outStr = Convert.ToBase64String(msEncrypt.ToArray());
				}
			}
			finally
			{
				// Clear the RijndaelManaged object.
				if (aesAlg != null)
					aesAlg.Clear();
			}

			// Return the encrypted bytes from the memory stream.
			return outStr;
		}

		/// <summary>
		/// Decrypt the given string.  Assumes the string was encrypted using 
		/// EncryptStringAES(), using an identical sharedSecret.
		/// </summary>
		/// <param name="cipherText">The text to decrypt.</param>
		/// <param name="sharedSecret">A password used to generate a key for decryption.</param>
		public static string DecryptStringAES(string cipherText, string sharedSecret)
		{
			if (string.IsNullOrEmpty(cipherText))
				throw new ArgumentNullException("cipherText");
			if (string.IsNullOrEmpty(sharedSecret))
				throw new ArgumentNullException("sharedSecret");

			// Declare the RijndaelManaged object
			// used to decrypt the data.
			RijndaelManaged aesAlg = null;

			// Declare the string used to hold
			// the decrypted text.
			string plaintext = null;

			try
			{
				// generate the key from the shared secret and the salt
				Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

				// Create a RijndaelManaged object
				// with the specified key and IV.
				aesAlg = new RijndaelManaged();
				aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
				aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

				// Create a decrytor to perform the stream transform.
				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
				// Create the streams used for decryption.                
				byte[] bytes = Convert.FromBase64String(cipherText);
				using (MemoryStream msDecrypt = new MemoryStream(bytes))
				{
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader srDecrypt = new StreamReader(csDecrypt))

							// Read the decrypted bytes from the decrypting stream
							// and place them in a string.
							plaintext = srDecrypt.ReadToEnd();
					}
				}
			}
			finally
			{
				// Clear the RijndaelManaged object.
				if (aesAlg != null)
					aesAlg.Clear();
			}

			return plaintext;
		}
	}
}
