using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.PersistenceSupport;

namespace Explora_Precios.Core.DataInterfaces
{
    public interface IProductLogRepository : IRepository<ProductLog>
    {
        void Insert(ProductLog productLog);
    }
}
