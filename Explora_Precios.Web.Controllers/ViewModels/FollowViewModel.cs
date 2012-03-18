using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explora_Precios.Web.Controllers.ViewModels
{
    public class FollowViewModel
    {
        public FollowViewModel()
        {
        }

        public bool Price { get; set; }
        public bool Offer { get; set; }
        public string Product { get; set; }
    }
}
