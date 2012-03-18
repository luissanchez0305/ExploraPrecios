using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.PersistenceSupport;

namespace Explora_Precios.Core.DataInterfaces
{
    public interface ICategoryRepository: IRepository<Category>
    {
        IList<Category> GetByDepartament(Department departament);
        Category GetBySubCategory(SubCategory subCategory);
        //List<Category> GetByDepartamentName(string depName);
        void DeleteCategoryItem(int id);
    }
}
