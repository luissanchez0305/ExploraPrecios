using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explora_Precios.Web.Controllers.ViewModels
{
    public class DepartmentViewModel
    {
        public string departmentTitle { get; set; }
        public int departmentId { get; set; }
        public List<CategoryViewModel> categories { get; set; }
    }
}
