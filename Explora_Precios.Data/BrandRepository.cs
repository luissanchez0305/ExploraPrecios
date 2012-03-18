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
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        public Brand GetByBrandName(string name)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(Brand))
                .Add(Expression.Like("name", name.ToLower(), MatchMode.Exact))
                .UniqueResult<Brand>();
        }
    }
}
