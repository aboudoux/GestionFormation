using GestionFormation.CoreDomain.Formations.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formations.Projections
{
    public class FormationSqlProjection : IProjectionHandler,
        IEventHandler<FormationCreated>,
        IEventHandler<FormationUpdated>,
        IEventHandler<FormationDeleted>
    {
        public void Handle(FormationCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Formations.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new FormationSqlEntity();
                    context.Formations.Add(entity);
                }
                
                entity.FormationId = @event.AggregateId;
                entity.Nom = @event.Nom;
                entity.Places = @event.Places;
                
                context.SaveChanges();
            }
        }

        public void Handle(FormationUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Formations.Find(@event.AggregateId);
                if (entity == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Stagiaires");

                entity.Nom = @event.Nom;
                entity.Places = @event.Places;
                context.SaveChanges();
            }
        }

        public void Handle(FormationDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new FormationSqlEntity() { FormationId = @event.AggregateId };
                context.Formations.Attach(entity);
                context.Formations.Remove(entity);
                context.SaveChanges();
            }
        }
    }
}