using System;
using GestionFormation.CoreDomain.Seats.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Seats.Projections
{
    public class SeatSqlProjection : IProjectionHandler,
        IEventHandler<SeatCreated>, 
        IEventHandler<SeatCanceled>, 
        IEventHandler<SeatRefused>, 
        IEventHandler<SeatValided>,
        IEventHandler<AgreementAssociated>        
    {
        public void Handle(SeatCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Seats.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new SeatSqlentity();
                    context.Seats.Add(entity);
                }

                entity.SeatId = @event.AggregateId;
                entity.SessionId = @event.SessionId;
                entity.CompanyId = @event.CompanyId;
                entity.StudentId = @event.StudentId;
                entity.Reason = "";
                entity.Status = SeatStatus.ToValidate;
                context.SaveChanges();
            }
        }

        public void Handle(SeatCanceled @event)
        {
            UpdateStatus(@event.AggregateId, SeatStatus.Canceled, @event.Reason);
        }

        public void Handle(SeatRefused @event)
        {
            UpdateStatus(@event.AggregateId, SeatStatus.Refused, @event.Reason);
        }

        public void Handle(SeatValided @event)
        {
            UpdateStatus(@event.AggregateId, SeatStatus.Valid);
        }      

        private void UpdateStatus(Guid placeId , SeatStatus status, string reason = "")
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var place = context.Seats.Find(placeId);
                if (place == null)
                    throw new EntityNotFoundException(placeId, "Place");
                place.Status = status;
                place.Reason = reason;
                context.SaveChanges();
            }
        }

        public void Handle(AgreementAssociated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var place = context.Seats.Find(@event.AggregateId);
                if (place == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Place");
                place.AssociatedAgreementId = @event.AgreementId;
                context.SaveChanges();
            }
        }       
    }
}
