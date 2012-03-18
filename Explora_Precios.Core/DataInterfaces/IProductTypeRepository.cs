using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.PersistenceSupport;

namespace Explora_Precios.Core.DataInterfaces
{
    public interface IProductTypeRepository : IRepository<ProductType>
    {
        IList<ProductType> GetBySubCategory(SubCategory subCategory);
        void DeleteProductTypeItem(int id);
    }
}
