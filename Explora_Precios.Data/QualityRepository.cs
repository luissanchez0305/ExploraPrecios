using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;
using SharpArch.Data.NHibernate;
using Explora_Precios.Core.DataInterfaces;
using NHibernate.Criterion;

namespace Explora_Precios.Data
{
    public class QualityRepository : Repository<Quality>, IQualityRepository
    {
        public Quality getByName(string name)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(Quality))
                .Add(Expression.Eq("name", name.ToLower()))
                .UniqueResult<Quality>();
        }
    }
}
