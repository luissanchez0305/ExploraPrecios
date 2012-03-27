using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Sgml;
using System.Net;

namespace Explora_Precios.ApplicationServices
{
    public class CommonUtilities
    {
        /* buscar a en b o b en a. El acercamiento en la similitud se da en el parametro atLeast
         * Si ambos tienen el mismo tamaño entonces la busqueda por defecto es de a en b */
        public static bool Contain(string a, string b, int atLeast)
        {
            var response = false;
            var aArray = a.ToList();
            var bArray = b.ToList();

            var aIndex = 0;
            aArray.ForEach(delegate(char aChar)
            {
                if (!response)
                {
                    var aTempIndex = aIndex;
                    var searchChar = aChar;
                    var bIndex = 0;
                    var matchCount = 0;
                    //while (searchChar == bArray[bIndex] && aArray.Count - aTempIndex >= bArray.Count)
                    while (searchChar == bArray[bIndex] && aTempIndex < aArray.Count -1)
                    {
                        bIndex++;
                        aTempIndex++;
                        searchChar = aArray[aTempIndex];
                        matchCount++;
                    }
                    response = matchCount >= (aArray.Count - atLeast);
                }
                aIndex++;
            });

            return response;

            //var shortArray = a.Length <= b.Length ? a.ToList() : b.ToList();
            //var bigArray = a.Length > b.Length ? a.ToList() : b.ToList();
            //var initialIndexArray = new List<int>();
            //var i = 0;
            ///* se obtiene una lista de enteros que representa los indices donde se encuentra el primer caracter
            //   del string corto en el string largo */
            //bigArray.ForEach(delegate(char caracter) { if (caracter == shortArray[0] && i < bigArray.Count - i + 1) initialIndexArray.Add(i); i++; });

            //var matchCountArray = new List<int>();
            //foreach (var initialIndex in initialIndexArray)
            //{
            //    var matchCount = 0;
            //    i = 0;
            //    while (i < shortArray.Count - 1)
            //    {
            //        i++;
            //        var searchChar = shortArray[i];
            //        if (searchChar == bigArray[initialIndex + i]) matchCount++; else break;
            //    }
            //    matchCountArray.Add(matchCount);
            //}

            //var response = "";
            //matchCountArray.ForEach(delegate(int matchCount) { if (matchCount >= (shortArray.Count - atLeast)) response = shortArray.ToString(); });
            //return response.Length > 0;
        }

        public static int Levenshtein(String a, String b)
        {
            if (string.IsNullOrEmpty(a))
            {
                if (!string.IsNullOrEmpty(b))
                {
                    return b.Length;
                }
                return 0;
            }

            if (string.IsNullOrEmpty(b))
            {
                if (!string.IsNullOrEmpty(a))
                {
                    return a.Length;
                }
                return 0;
            }

            Int32 cost;
            Int32[,] d = new int[a.Length + 1, b.Length + 1];
            Int32 min1;
            Int32 min2;
            Int32 min3;

            for (Int32 i = 0; i <= d.GetUpperBound(0); i += 1)
            {
                d[i, 0] = i;
            }

            for (Int32 i = 0; i <= d.GetUpperBound(1); i += 1)
            {
                d[0, i] = i;
            }

            for (Int32 i = 1; i <= d.GetUpperBound(0); i += 1)
            {
                for (Int32 j = 1; j <= d.GetUpperBound(1); j += 1)
                {
                    cost = Convert.ToInt32(!(a[i - 1] == b[j - 1]));

                    min1 = d[i - 1, j] + 1;
                    min2 = d[i, j - 1] + 1;
                    min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];
        }

        public static string CacheImage(Byte[] ImageArray)
        {
            Guid id = Guid.NewGuid();
            System.Web.HttpRuntime.Cache.Insert(id.ToString(), ImageArray,
                null,
                DateTime.Now.AddMinutes(2),
                System.Web.Caching.Cache.NoSlidingExpiration);
            return id.ToString();
        }

        public string GetXHTML(string address)
        {
            StringWriter _writer = new StringWriter();
            XmlTextWriter _xmlwriter = new XmlTextWriter(_writer);
            SgmlReader reader = new SgmlReader();
            string result = string.Empty;

            try
            {
                StreamReader responseReader = new StreamReader(GetUrlPageResponse(address).GetResponseStream()); ;
                string ResponseRquest = responseReader.ReadToEnd();

                reader.DocType = "HTML";
                reader.InputStream = new StringReader(ResponseRquest);
                _xmlwriter.Formatting = Formatting.None;

                while (reader.Read())
                {
                    if ((reader.NodeType != XmlNodeType.DocumentType) && (reader.NodeType != XmlNodeType.Whitespace))
                    {
                        _xmlwriter.WriteNode(reader, true);
                    }
                }

                result = _writer.ToString();
            }
            catch (WebException e)
            {
                if (!e.Message.Contains("404"))
                    result = _writer.ToString();
            }
            catch { result = _writer.ToString(); }
            finally 
            {
                reader.Close();
                _xmlwriter.Close();
            }

            return result;
        }

        public static WebResponse GetUrlPageResponse(string address)
        {
            HttpWebRequest webRequest = WebRequest.Create(address) as HttpWebRequest;
            return webRequest.GetResponse();
        }

        public static Stream GetImageFromUrl(string address)
        {
            WebRequest req = WebRequest.Create(address);
            WebResponse response = req.GetResponse();
            return response.GetResponseStream();
        }
    }
}
