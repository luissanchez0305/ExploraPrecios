using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;

namespace Explora_Precios.Web.Controllers.ViewModels
{
    public class SubCategoryViewModel
    {
        public int categoryId { get; set; }
        public int subCategoryId { get; set; }
        public string subCategoryTitle { get; set; }
        //public Catalog.Types catalogType { get { return Catalog.Types.SubCategory; } }
        public List<ProductTypeViewModel> productTypes { get; set; }
    }
}
