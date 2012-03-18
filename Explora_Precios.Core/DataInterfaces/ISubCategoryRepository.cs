using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.PersistenceSupport;

namespace Explora_Precios.Core.DataInterfaces
{
    public interface ISubCategoryRepository : IRepository<SubCategory>
    {
        SubCategory GetByProductType(ProductType productType);
        IList<SubCategory> GetByCategory(Category category);
        void DeleteSubCategoryItem(int id);
    }
}
