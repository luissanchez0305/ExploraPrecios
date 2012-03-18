using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.PersistenceSupport;

namespace Explora_Precios.Core.DataInterfaces
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        void DeleteDepartmentItem(int id);
        Department getByCategory(Category category);
    }
}
