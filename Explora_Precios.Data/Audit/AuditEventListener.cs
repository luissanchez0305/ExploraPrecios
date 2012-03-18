using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Event;
using System.Security.Principal;
using NHibernate.Persister.Entity;
using Explora_Precios.Core;
using SharpArch.Data.NHibernate;
using NHibernate.Criterion;
using SharpArch.Core.DomainModel;

namespace Explora_Precios.Data.Audit
{
    public class AuditEventListener
    {
        private NHibernate.ISession _session = null;
        public NHibernate.ISession Session
        {
            get
            {
                if (_session == null)
                {
                    _session = NHibernateSession.GetDefaultSessionFactory().OpenSession();
                }
                return _session;
            }
        }

        protected bool IsAuditable(Type entityType)
        {
            return Attribute.IsDefined(entityType, typeof(Explora_Precios.Core.Helper.AuditableAttribute));
        }

        protected Dictionary<string, object> MakeStateDictionary(IEntityPersister persister, object[] entityState)
        {
            var response = new Dictionary<string, object>();

            for (var i = 0; i < persister.PropertyNames.Length; i++)
            {
                response.Add(persister.PropertyNames[i], entityState[i]);
            }

            return response;
        }

        protected void InsertLogEntry(User user, ProductLog.ProductLogEntryOperations operationType, int entityId, Type entityType, Dictionary<string, object> entityData, object entityObj)
        {
            var logEntry = new ProductLog() { 
                //client_ProductId = entityId,
                changeDate = DateTime.Now,
                operation = operationType,
                user_Id = user.Id
            };
            Session.Save(logEntry);
            Session.Flush();
        }
    }

    public class UpdateEventListener : AuditEventListener, IPostUpdateEventListener
    {
        public void OnPostUpdate(PostUpdateEvent @event)
        {
            if (!IsAuditable(@event.Entity.GetType()))
                return;

            var user = Session.CreateCriteria(typeof(User))
                .Add(Restrictions.Eq("Id", 1))
                .UniqueResult<User>();
            if (user == null)
                return;

            InsertLogEntry(user, ProductLog.ProductLogEntryOperations.Update,
                ((Entity)@event.Entity).Id,
                @event.Entity.GetType(),
                MakeStateDictionary(@event.Persister, @event.State),
                @event.Entity);
        }
    }

    public class InsertEventListener : AuditEventListener, IPostInsertEventListener
    {
        public void OnPostInsert(PostInsertEvent @event)
        {
            if (!IsAuditable(@event.Entity.GetType()))
                return;

            var user = Session.CreateCriteria(typeof(User))
                .Add(Restrictions.Eq("Id", 1))
                .UniqueResult<User>();
            if (user == null)
                return;

            InsertLogEntry(user, ProductLog.ProductLogEntryOperations.Insert,
                ((Entity)@event.Entity).Id,
                @event.Entity.GetType(),
                MakeStateDictionary(@event.Persister, @event.State),
                @event.Entity);
        }
    }
}
