using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;
using Explora_Precios.Core.DataInterfaces;
using SharpArch.Data.NHibernate;

namespace Explora_Precios.Data
{
    public class ProductLogRepository : Repository<ProductLog>, IProductLogRepository
    {
        public void Insert(ProductLog productLog)
        {
            NHibernate.ISession session = SharpArch.Data.NHibernate.NHibernateSession.Current;

            using (NHibernate.ITransaction transaction = session.BeginTransaction())
            {

                session.Save(productLog);
                transaction.Commit();
            }            
        }
    }
}
