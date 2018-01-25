using GestionFormation.CoreDomain.Formateurs.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formateurs.Projections
{
    public class FormateurSqlProjection : IProjectionHandler,
        IEventHandler<FormateurCreated>,
        IEventHandler<FormateurUpdated>,
        IEventHandler<FormateurDeleted>
    {
        public void Handle(FormateurCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Formateurs.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new FormateurSqlEntity();
                    context.Formateurs.Add(entity);
                }

                entity.FormateurId = @event.AggregateId;
                entity.Nom = @event.Nom;
                entity.Prenom = @event.Prenom;
                entity.Email = @event.Email;
                context.SaveChanges();
            }
        }

        public void Handle(FormateurUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var formateurToUpdate = context.Formateurs.Find(@event.AggregateId);
                if( formateurToUpdate == null )
                    throw new EntityNotFoundException(@event.AggregateId, "Formateur");

                formateurToUpdate.Nom = @event.Nom;
                formateurToUpdate.Prenom = @event.Prenom;
                formateurToUpdate.Email = @event.Email;
                context.SaveChanges();
            }
        }

        public void Handle(FormateurDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new FormateurSqlEntity(){ FormateurId = @event.AggregateId };
                context.Formateurs.Attach(entity);
                context.Formateurs.Remove(entity);
                context.SaveChanges();
            }
        }
    }
}
