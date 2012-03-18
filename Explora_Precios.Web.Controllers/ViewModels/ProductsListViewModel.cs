using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core.Helper;
using Explora_Precios.Core;

namespace Explora_Precios.Web.Controllers.ViewModels
{
    public class ProductsListViewModel
    {
        public ProductsListViewModel()
        {
            products = new PagedList<Product>(new List<Product>(), 0, int.Parse(System.Configuration.ConfigurationManager.AppSettings["DefaultPageSize"]));
        }
        public IEnumerable<ProductViewModel> productsList { get; set; }
        public IPagedList<Product> products { get; set; }

    }
}
