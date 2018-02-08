using GestionFormation.CoreDomain.Companies.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Companies.Projections
{
    public class CompanySqlProjection : IProjectionHandler,
        IEventHandler<CompanyCreated>,
        IEventHandler<CompanyUpdated>,
        IEventHandler<CompanyDeleted>
    {
        public void Handle(CompanyCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Companies.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new CompanySqlEntity();
                    context.Companies.Add(entity);
                }

                entity.CompanyId = @event.AggregateId;
                entity.Name = @event.Name;
                entity.Address = @event.Address;
                entity.ZipCode = @event.ZipCode;
                entity.City = @event.City;
                context.SaveChanges();
            }
        }

        public void Handle(CompanyUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Companies.Find(@event.AggregateId);
                if( entity == null )
                    throw new EntityNotFoundException(@event.AggregateId, "Societe");
                entity.Name = @event.Name;
                entity.Address = @event.Address;
                entity.ZipCode = @event.ZipCode;
                entity.City = @event.City;
                context.SaveChanges();
            }
        }

        public void Handle(CompanyDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.GetEntity<CompanySqlEntity>(@event.AggregateId);
                entity.Removed = true;
                context.SaveChanges();
            }
        }
    }
}
