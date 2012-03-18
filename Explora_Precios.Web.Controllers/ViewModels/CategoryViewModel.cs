using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explora_Precios.Web.Controllers.ViewModels
{
    public class CategoryViewModel
    {
        public int departmentId { get; set; }
        public int categoryId { get; set; }
        public string categoryTitle { get; set; }
        public List<SubCategoryViewModel> subCategories { get; set; }
    }
}
