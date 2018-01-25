using GestionFormation.CoreDomain.Stagiaires.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Stagiaires.Projections
{
    public class StagiaireSqlProjection : IProjectionHandler,
        IEventHandler<StagiaireCreated>, 
        IEventHandler<StagiaireUpdated>, 
        IEventHandler<StagiaireDeleted>
    {
        public void Handle(StagiaireCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Stagiaires.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new StagiaireSqlEntity();
                    context.Stagiaires.Add(entity);
                }

                entity.StagiaireId = @event.AggregateId;
                entity.Nom = @event.Nom;
                entity.Prenom = @event.Prenom;                

                context.SaveChanges();
            }
        }

        public void Handle(StagiaireUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Stagiaires.Find(@event.AggregateId);
                if (entity == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Stagiaires");
                entity.Nom = @event.Nom;
                entity.Prenom = @event.Prenom;                
                context.SaveChanges();
            }
        }

        public void Handle(StagiaireDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new StagiaireSqlEntity() {StagiaireId = @event.AggregateId};
                context.Stagiaires.Attach(entity);
                context.Stagiaires.Remove(entity);
                context.SaveChanges();
            }
        }
    }
}