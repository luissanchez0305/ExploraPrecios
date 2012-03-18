using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;

namespace Explora_Precios.Web.Controllers.ViewModels
{
    public class ProductTypeViewModel
    {
        public int subCategoryId { get; set; }
        public int productTypeId { get; set; }
        public string productTypeTitle { get; set; }
        //public Catalog.Types catalogType { get { return Catalog.Types.ProductType; } }
    }
}
