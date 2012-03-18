using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Data.NHibernate;
using Explora_Precios.Core;
using Explora_Precios.Core.DataInterfaces;
using NHibernate.Criterion;

namespace Explora_Precios.Data
{
    public class Product_QualityRepository : Repository<Product_Quality>, IProduct_QualityRepository
    {
        public Product_Quality getByProductQuality(Product product, Quality quality)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(Product_Quality))
                .Add(Expression.And(Expression.Eq("product", product), Expression.Eq("quality", quality)))
                .UniqueResult<Product_Quality>();
        }
    }
}
