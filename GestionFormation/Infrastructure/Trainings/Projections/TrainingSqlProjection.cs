using System;
using GestionFormation.CoreDomain.Trainings.Events;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure.Trainings.Projections
{
    public class TrainingSqlProjection : IProjectionHandler,
        IEventHandler<TrainingCreated>,
        IEventHandler<TrainingUpdated>,
        IEventHandler<TrainingDeleted>,
        IEventHandler<TrainingDisabled>
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
                entity.Color = @event.Color;
                
                context.SaveChanges();
            }
        }

        public void Handle(TrainingUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.GetEntity<TrainingSqlEntity>(@event.AggregateId);

                entity.Name = @event.Name;
                entity.Seats = @event.Seats;
                entity.Color = @event.Color;
                context.SaveChanges();
            }
        }

        public void Handle(TrainingDeleted @event)
        {
            DisableTraining(@event.AggregateId);
        }

        public void Handle(TrainingDisabled @event)
        {
            DisableTraining(@event.AggregateId);
        }

        private void DisableTraining(Guid trainingId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.GetEntity<TrainingSqlEntity>(trainingId);
                entity.Removed = true;
                context.SaveChanges();
            }
        }
    }
}