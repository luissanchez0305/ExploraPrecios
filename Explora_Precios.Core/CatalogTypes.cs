using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explora_Precios.Core
{
    public class Catalog
    {
        [Flags]
        public enum Types
        { 
            Department = 0,
            Category = 1,
            SubCategory = 2,
            ProductType = 3
        }
    }
}
