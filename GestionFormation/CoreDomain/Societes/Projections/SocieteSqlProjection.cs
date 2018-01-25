using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Societes.Projections
{
    public class SocieteSqlProjection : IProjectionHandler,
        IEventHandler<SocieteCreated>,
        IEventHandler<SocieteUpdated>,
        IEventHandler<SocieteDeleted>
    {
        public void Handle(SocieteCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Societes.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new SocieteSqlEntity();
                    context.Societes.Add(entity);
                }

                entity.SocieteId = @event.AggregateId;
                entity.Nom = @event.Nom;
                entity.Addresse = @event.Addresse;
                entity.CodePostal = @event.Codepostal;
                entity.Ville = @event.Ville;
                context.SaveChanges();
            }
        }

        public void Handle(SocieteUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Societes.Find(@event.AggregateId);
                if( entity == null )
                    throw new EntityNotFoundException(@event.AggregateId, "Societe");
                entity.Nom = @event.Nom;
                entity.Addresse = @event.Addresse;
                entity.CodePostal = @event.Codepostal;
                entity.Ville = @event.Ville;
                context.SaveChanges();
            }
        }

        public void Handle(SocieteDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new SocieteSqlEntity() {SocieteId = @event.AggregateId};
                context.Societes.Attach(entity);
                context.Societes.Remove(entity);
                context.SaveChanges();
            }
        }
    }
}
