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
                entity.Nom = @event.Nom;
                entity.Prenom = @event.Prenom;
                entity.Email = @event.Email;
                entity.Telephone = @event.Telephone;
                entity.SocieteId = @event.SocieteId;
                
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
                entity.Nom = @event.Nom;
                entity.Prenom = @event.Prenom;
                entity.Email = @event.Email;
                entity.Telephone = @event.Telephone;
                entity.SocieteId = @event.SocieteId;
                context.SaveChanges();
            }
        }

        public void Handle(ContactDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new ContactSqlEntity() { ContactId = @event.AggregateId };
                context.Contacts.Attach(entity);
                context.Contacts.Remove(entity);
                context.SaveChanges();
            }
        }
    }
}
