using GestionFormation.CoreDomain.Sessions.Events;
using GestionFormation.CoreDomain.Trainings.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Projections
{
    public class SessionSqlProjection : IProjectionHandler,
        IEventHandler<SessionPlanned>, 
        IEventHandler<SessionUpdated>, 
        IEventHandler<SessionDeleted>,
        IEventHandler<SessionCanceled>, 
        IEventHandler<TrainingDeleted>
    {
        public void Handle(SessionPlanned @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Sessions.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new SessionSqlEntity();
                    context.Sessions.Add(entity);
                }

                entity.TrainingId = @event.TrainingId;
                entity.SessionId = @event.AggregateId;
                entity.SessionStart = @event.SessionStart;
                entity.Duration = @event.Duration;
                entity.TrainerId = @event.TrainerId;
                entity.LocationId = @event.LocationId;
                entity.Seats = @event.Seats;
                entity.ReservedSeats = 0;
                context.SaveChanges();
            }
        }

        public void Handle(SessionUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Sessions.Find(@event.AggregateId);
                if( entity == null )
                    throw new EntityNotFoundException(@event.AggregateId, "Session");

                entity.TrainingId = @event.TrainingId;
                entity.SessionId = @event.AggregateId;
                entity.SessionStart = @event.SessionStart;
                entity.Duration = @event.Duration;
                entity.TrainerId = @event.TrainerId;
                entity.LocationId = @event.LocationId;
                entity.Seats = @event.Seats;                
                context.SaveChanges();
            }
        }

        public void Handle(SessionDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new SessionSqlEntity(){ SessionId = @event.AggregateId};
                context.Sessions.Attach(entity);
                context.Sessions.Remove(entity);
                context.SaveChanges();
            }
        }

        public void Handle(SessionCanceled @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Sessions.Find(@event.AggregateId);
                if (entity == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Session");
                entity.Canceled = true;
                entity.CancelReason = @event.Raison;
                context.SaveChanges();
            }
        }

        public void Handle(TrainingDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                context.Database.ExecuteSqlCommand($"DELETE FROM dbo.SESSION WHERE TrainerId = '{@event.AggregateId}'");
            }
        }
    }
}
