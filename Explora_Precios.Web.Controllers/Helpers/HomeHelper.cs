using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using System.Net;
using System.IO;
using Facebook;

namespace Explora_Precios.Web.Controllers.Helpers
{
	public static class HomeHelper
	{
		static Random _random = new Random();

		public static IEnumerable<T> RandomizeList<T>(this IEnumerable<T> arr) 
		{
			List<KeyValuePair<int, T>> list = new List<KeyValuePair<int, T>>();
			// Add all strings from array
			// Add new random int each time
			foreach (var s in arr)
			{
				list.Add(new KeyValuePair<int, T>(_random.Next(), s));
			}
			// Sort the list by the random number
			var sorted = from item in list
						 orderby item.Key
						 select item;
			// Allocate new string array
			var result = new T[arr.Count()];
			// Copy values to array
			int index = 0;
			foreach (var pair in sorted)
			{
				result[index] = pair.Value;
				index++;
			}
			// Return copied array
			return result;
		}

		public static int ExactMinValue(this float val)
		{
			return ExactMinValue((int)val);
		}

		public static int ExactMinValue(this int val)
		{
			if (val >= 10)
			{
				var ValStr = val.ToString();
				return int.Parse(new String((new char[] { ValStr.First() }).Concat(ValStr.Skip(1).Select(digit => '0').ToArray()).ToArray()));
			}
			return 0;
		}

		public static int[] FitImage(this byte[] data, int fitWidth, int fitHeight)
		{
			System.Drawing.Image realImage;
			var width = 0;
			var height = 0;

			using (System.IO.MemoryStream stream = new System.IO.MemoryStream(data))
			{
				realImage = System.Drawing.Image.FromStream(stream);
			}

			if (fitWidth < realImage.Width)
			{
				var proportion = (Convert.ToDecimal(fitWidth) / Convert.ToDecimal(realImage.Width));
				width = fitWidth;
				height = (int)(realImage.Height * proportion);
				if (height > fitHeight)
				{
					proportion = Convert.ToDecimal(fitHeight) / Convert.ToDecimal(height);
					height = fitHeight;
					width = (int)(fitWidth * proportion);
				}
			}
			else if (fitHeight < realImage.Height)
			{
				var proportion = (Convert.ToDecimal(fitHeight) / Convert.ToDecimal(realImage.Height));
				width = (int)(realImage.Width * proportion);
				height = fitHeight;
				if (width > fitWidth)
				{
					proportion = Convert.ToDecimal(fitWidth) / Convert.ToDecimal(width);
					height = (int)(fitHeight * proportion);
					width = fitWidth;
				}
			}
			else if (realImage.Height < fitHeight)
			{
				var proportion = (Convert.ToDecimal(realImage.Height) / Convert.ToDecimal(fitHeight));
				width = (int)(realImage.Width * proportion);
				height = fitHeight;
			}
			else
			{
				var proportion = (Convert.ToDecimal(realImage.Width) / Convert.ToDecimal(fitWidth));
				width = fitWidth;
				height = (int)(realImage.Height * proportion);
			}

			return new[] { width, height };
		}

	}

}
