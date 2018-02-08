using GestionFormation.CoreDomain.Contacts.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Contacts.Projections
{
    public class ContactSqlProjection : IProjectionHandler,
        IEventHandler<ContactCreated>,
        IEventHandler<ContactUpdated>,
        IEventHandler<ContactDeleted>
    {
        public void Handle(ContactCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Contacts.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new ContactSqlEntity();
                    context.Contacts.Add(entity);
                }

                entity.ContactId = @event.AggregateId;
                entity.Lastname = @event.Lastname;
                entity.Firstname = @event.Firstname;
                entity.Email = @event.Email;
                entity.Telephone = @event.Telephone;
                entity.CompanyId = @event.CompanyId;
                
                context.SaveChanges();
            }
        }

        public void Handle(ContactUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Contacts.Find(@event.AggregateId);
                if( entity == null )
                    throw new EntityNotFoundException(@event.AggregateId, "Contact");
                entity.Lastname = @event.Lastname;
                entity.Firstname = @event.Firstname;
                entity.Email = @event.Email;
                entity.Telephone = @event.Telephone;
                entity.CompanyId = @event.CompanyId;
                context.SaveChanges();
            }
        }

        public void Handle(ContactDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.GetEntity<ContactSqlEntity>(@event.AggregateId);
                entity.Removed = true;
                context.SaveChanges();
            }
        }
    }
}
