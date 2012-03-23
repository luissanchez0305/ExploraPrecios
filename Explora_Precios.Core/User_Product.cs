using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;

namespace Explora_Precios.Core
{
    public class User_Product : Entity
    {
        public enum RelationType { Rating = 1, FollowIsOnSale = 2, FollowLessThanPrice = 3, FollowDoesPriceWentDown = 4, Liked = 5 }

        public User_Product() { Notified = DateTime.Parse("05/03/1976"); }

        [DomainSignature]
        public virtual User user { get; set; }
        public virtual Product product { get; set; }
        public virtual float value { get; set; }
        public virtual RelationType Type { get; set; }
        public virtual DateTime Notified { get; set; }
        public virtual float NotifiedValue { get; set; }
    }
}
