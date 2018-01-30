using GestionFormation.CoreDomain.Trainings.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainings.Projections
{
    public class TrainingSqlProjection : IProjectionHandler,
        IEventHandler<TrainingCreated>,
        IEventHandler<TrainingUpdated>,
        IEventHandler<TrainingDeleted>
    {
        public void Handle(TrainingCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Trainings.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new TrainingSqlEntity();
                    context.Trainings.Add(entity);
                }
                
                entity.TrainingId = @event.AggregateId;
                entity.Name = @event.Name;
                entity.Seats = @event.Seats;
                
                context.SaveChanges();
            }
        }

        public void Handle(TrainingUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Trainings.Find(@event.AggregateId);
                if (entity == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Stagiaires");

                entity.Name = @event.Name;
                entity.Seats = @event.Seats;
                context.SaveChanges();
            }
        }

        public void Handle(TrainingDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new TrainingSqlEntity() { TrainingId = @event.AggregateId };
                context.Trainings.Attach(entity);
                context.Trainings.Remove(entity);
                context.SaveChanges();
            }
        }
    }
}