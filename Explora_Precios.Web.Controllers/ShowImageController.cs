using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;

namespace Explora_Precios.Web.Controllers
{
    public class ShowImageController : Controller
    {
        public void Index()
        {
            var cacheImageId = HttpRuntime.Cache[Request.QueryString["image"]];
            if (cacheImageId != null)
            {
                Byte[] image = (Byte[])cacheImageId;
                Response.ContentType = "image/jpg";
                Response.BinaryWrite(image);
            }
        }
    }
}
