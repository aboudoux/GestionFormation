using GestionFormation.CoreDomain.Conventions.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Conventions.Projections
{
    public class ConventionSqlProjection : IProjectionHandler,
        IEventHandler<ConventionCreated>,
        IEventHandler<ConventionSigned>,
        IEventHandler<ConventionRevoked>
    {
        public void Handle(ConventionCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Conventions.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new ConventionSqlEntity();
                    context.Conventions.Add(entity);
                }

                entity.ConventionId = @event.AggregateId;
                entity.ContactId = @event.ContactId;
                entity.ConventionNumber = @event.Convention;
                entity.TypeConvention = @event.TypeConvention;
                
                context.SaveChanges();
            }
        }

        public void Handle(ConventionSigned @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Conventions.Find(@event.AggregateId);
                if( entity == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Convention");
                entity.DocumentId = @event.DocumentId;
                context.SaveChanges();
            }
        }

        public void Handle(ConventionRevoked @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new ConventionSqlEntity() { ConventionId = @event.AggregateId };
                context.Conventions.Attach(entity);
                context.Conventions.Remove(entity);
                context.SaveChanges();
            }
        }
    }
}
