using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.PersistenceSupport;
using Explora_Precios.Core;

namespace Explora_Precios.Core.DataInterfaces
{
    public interface IProduct_QualityRepository : IRepository<Product_Quality>
    {
        Product_Quality getByProductQuality(Product product, Quality quality);

    }
}
