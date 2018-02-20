using System;
using GestionFormation.CoreDomain.Agreements.Events;
using GestionFormation.CoreDomain.Seats.Events;
using GestionFormation.CoreDomain.Sessions.Events;
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
        IEventHandler<AgreementAssociated>,
        IEventHandler<AgreementRevoked>,
        IEventHandler<MissingStudentReported>,
        IEventHandler<CertificatOfAttendanceSent>
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

        private void UpdateStatus(Guid seatId , SeatStatus status, string reason = "")
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var seat = context.GetEntity<SeatSqlentity>(seatId);
                seat.Status = status;
                seat.Reason = reason;
                context.SaveChanges();
            }
        }

        public void Handle(AgreementAssociated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var seat = context.GetEntity<SeatSqlentity>(@event.AggregateId);
                seat.AssociatedAgreementId = @event.AgreementId;
                seat.AgreementRevoked = false;
                context.SaveChanges();
            }
        }

        public void Handle(AgreementRevoked @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                context.Database.ExecuteSqlCommand($"UPDATE Seat SET AssociatedAgreementId = NULL, AgreementRevoked = 1 WHERE AssociatedAgreementId = '{@event.AggregateId}'");
            }
        }

        public void Handle(MissingStudentReported @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var seat = context.GetEntity<SeatSqlentity>(@event.AggregateId);
                seat.StudentMissing = true;
                context.SaveChanges();
            }
        }

        public void Handle(CertificatOfAttendanceSent @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var seat = context.GetEntity<SeatSqlentity>(@event.AggregateId);
                seat.CertificateOfAttendanceId = @event.DocumentId;
                context.SaveChanges();
            }
        }
    }
}
