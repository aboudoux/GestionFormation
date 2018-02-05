using System.Linq;
using GestionFormation.CoreDomain.Agreements.Projections;
using GestionFormation.CoreDomain.BookingNotifications.Events;
using GestionFormation.CoreDomain.Companies.Projections;
using GestionFormation.CoreDomain.Students.Projections;
using GestionFormation.CoreDomain.Users;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.BookingNotifications.Projections
{
    public class BookingNotificationSqlProjections : IProjectionHandler,
        IEventHandler<SeatToValidateSent>,
        IEventHandler<AgreementToCreateSent>,    
        IEventHandler<AgreementToSignSent>,
        IEventHandler<BookingNotificationRemoved>

    {
        public void Handle(SeatToValidateSent @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.BookingNotifications.FirstOrDefault(a => a.Id == @event.AggregateId);
                if (entity == null)
                {
                    entity = new BookingNotificationSqlEntity();
                    context.BookingNotifications.Add(entity);
                }

                var seat = context.Seats.Find(@event.SeatId);
                if(seat == null)
                    throw new EntityNotFoundException(@event.SeatId, "Seat");

                var student = context.GetEntity<StudentSqlEntity>(seat.StudentId);

                entity.Id = @event.AggregateId;
                entity.SeatId = @event.SeatId;
                entity.SessionId = @event.SessionId;
                entity.CompanyId = @event.CompanyId;
                entity.Label = $"Place de {student.Lastname} {student.Firstname} à valider.";
                entity.AffectedRole = UserRole.Manager;
                entity.ReminderType = BookingNotificationType.PlaceToValidate;

                context.SaveChanges();
            }
        }

        public void Handle(AgreementToCreateSent @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.BookingNotifications.FirstOrDefault(a=>a.Id == @event.AggregateId);
                if (entity == null)
                {
                    entity = new BookingNotificationSqlEntity();
                    context.BookingNotifications.Add(entity);
                }

                var company = context.GetEntity<CompanySqlEntity>(@event.CompanyId);

                entity.Id = @event.AggregateId;
                entity.SessionId = @event.SessionId;
                entity.CompanyId = @event.CompanyId;
                entity.ReminderType = BookingNotificationType.AgreementToCreate;
                entity.AffectedRole = UserRole.Operator;
                entity.Label = $"{company.Name} - Convention à créer";

                context.SaveChanges();
            }
        }

        public void Handle(AgreementToSignSent @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.GetEntity<BookingNotificationSqlEntity>(@event.AggregateId);
                var company = context.GetEntity<CompanySqlEntity>(@event.CompanyId);

                entity.AgreementId = @event.AgreementId;
                entity.ReminderType = BookingNotificationType.AgreementToSign;
                entity.AffectedRole = UserRole.Operator;
                entity.Label = $"{company.Name} - Convention à retourner signée";

                context.SaveChanges();
            }
        }

        public void Handle(BookingNotificationRemoved @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new BookingNotificationSqlEntity(){ Id = @event.AggregateId };
                context.BookingNotifications.Attach(entity);
                context.BookingNotifications.Remove(entity);
                context.SaveChanges();
            }
        }
    }
}