using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.PersistenceSupport;

namespace Explora_Precios.Core.DataInterfaces
{
    public interface IUser_ProductRepository : IRepository<User_Product>
    {
        User_Product GetByUserAndProductAndType(User user, Product product, User_Product.RelationType type);
        IList<User_Product> GetByProductAndActive(Product product);
        void Update(User_Product FollowOffer, User_Product FollowPrice);
        void Update(IEnumerable<User_Product> FollowObj);
    }
}
