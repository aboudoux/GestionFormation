using GestionFormation.CoreDomain.Lieux.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Lieux.Projections
{
    public class LieuSqlProjection : IProjectionHandler,
        IEventHandler<LieuCreated>,
        IEventHandler<LieuUpdated>,
        IEventHandler<LieuDeleted>
    {
        public void Handle(LieuCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Lieux.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new LieuSqlEntity();
                    context.Lieux.Add(entity);
                }

                entity.Id = @event.AggregateId;
                entity.Nom = @event.Nom;
                entity.Addresse = @event.Addresse;
                entity.Actif = true;
                entity.Places = @event.Places;
                context.SaveChanges();
            }
        }

        public void Handle(LieuUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var lieu = context.Lieux.Find(@event.AggregateId);
                if( lieu == null )
                    throw new EntityNotFoundException(@event.AggregateId, "Lieu");

                lieu.Nom = @event.Nom;
                lieu.Addresse = @event.Addresse;
                lieu.Places = @event.Places;
                context.SaveChanges();
            }
        }

        public void Handle(LieuDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var lieu = context.Lieux.Find(@event.AggregateId);
                if (lieu == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Lieu");

                lieu.Actif = false;
                context.SaveChanges();
            }
        }
    }
}
