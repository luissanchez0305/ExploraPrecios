using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.PersistenceSupport;

namespace Explora_Precios.Core.DataInterfaces
{
    public interface IGroup_UserRepository : IRepository<Group_User>
    {
        IList<Group_User> GetByUser(User user);
        void Update(Group_User user);
    }
}
