using System.Web.Mvc;
using System;
using Explora_Precios.Data;
using SharpArch.Data;
using System.Linq;
using Explora_Precios.Data.NHibernateMaps;
using System.IO;
using Explora_Precios.Web.Controllers.ViewModels;
using Explora_Precios.Core.DataInterfaces;
using Explora_Precios.Core;
using Explora_Precios.Core.Helper;
using System.Collections.Generic;
using Explora_Precios.Web.Controllers.Helpers;
using Explora_Precios.ApplicationServices;
using System.Drawing.Imaging;
using System.Web;
using System.Net;

namespace Explora_Precios.Web.Controllers
{
    [HandleError]
    public class ImageController : PrimaryController
    {
        IProductRepository _productRepository;

        public ImageController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public ActionResult Index(string id)
        {
            var productObj = _productRepository.Get(id.ProductId());
            var productVM = new ProductViewModel { productImage = productObj.image.imageObj, productName = productObj.name };
            return View(productVM);
        }
    }
}