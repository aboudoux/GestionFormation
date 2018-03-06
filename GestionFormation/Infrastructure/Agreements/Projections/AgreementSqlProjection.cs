using GestionFormation.CoreDomain.Agreements.Events;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure.Agreements.Projections
{
    public class AgreementSqlProjection : IProjectionHandler,
        IEventHandler<AgreementCreated>,
        IEventHandler<AgreementSigned>,
        IEventHandler<AgreementRevoked>,
        IEventHandler<AgreementUpdated>
    {
        public void Handle(AgreementCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Agreements.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new AgreementSqlEntity();
                    context.Agreements.Add(entity);
                }

                entity.AgreementId = @event.AggregateId;
                entity.ContactId = @event.ContactId;
                entity.AgreementNumber = @event.Agreement;
                entity.AgreementTypeAgreement = @event.AgreementType;
                
                context.SaveChanges();
            }
        }

        public void Handle(AgreementSigned @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.GetEntity<AgreementSqlEntity>(@event.AggregateId);
                entity.DocumentId = @event.DocumentId;
                context.SaveChanges();
            }
        }

        public void Handle(AgreementRevoked @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new AgreementSqlEntity() { AgreementId = @event.AggregateId };
                context.Agreements.Attach(entity);
                context.Agreements.Remove(entity);
                context.SaveChanges();
            }
        }

        public void Handle(AgreementUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.GetEntity<AgreementSqlEntity>(@event.AggregateId);
                entity.PricePerDayAndPerStudent = @event.PricePerDayAndPerStudent;                
                entity.PackagePrice = @event.PackagePrice;
                context.SaveChanges();
            }
        }
    }
}
