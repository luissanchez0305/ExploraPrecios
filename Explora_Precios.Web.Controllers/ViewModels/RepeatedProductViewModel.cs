using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explora_Precios.Web.Controllers.ViewModels
{
    public class RepeatedProductsViewModel
    {
        public RepeatedProductsViewModel()
        {
            repeatedProducts = new List<RepeatedProductViewModel>();
        }

        public ProductViewModel masterProduct { get; set; }
        public List<RepeatedProductViewModel> repeatedProducts { get; set; }
    }

    public class RepeatedProductViewModel
    {
        public bool isChecked { get; set; }
        public ProductViewModel productRepeated { get; set; }
    }
}
