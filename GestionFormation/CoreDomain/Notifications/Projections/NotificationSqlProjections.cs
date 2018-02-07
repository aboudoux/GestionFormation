using System.Linq;
using GestionFormation.CoreDomain.Companies.Projections;
using GestionFormation.CoreDomain.Notifications.Events;
using GestionFormation.CoreDomain.Students.Projections;
using GestionFormation.CoreDomain.Users;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Notifications.Projections
{
    public class NotificationSqlProjections : IProjectionHandler,
        IEventHandler<SeatToValidateNotificationSent>,
        IEventHandler<AgreementToCreateNotificationSent>,    
        IEventHandler<AgreementToSignNotificationSent>,
        IEventHandler<NotificationRemoved>,
        IEventHandler<NotificationManagerCreated>
    {
        public void Handle(SeatToValidateNotificationSent @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Notifications.FirstOrDefault(a => a.Id == @event.NotificationId);
                if (entity == null)
                {
                    entity = new NotificationSqlEntity();
                    context.Notifications.Add(entity);
                }

                var seat = context.Seats.Find(@event.SeatId);
                if(seat == null)
                    throw new EntityNotFoundException(@event.SeatId, "Seat");

                var student = context.GetEntity<StudentSqlEntity>(seat.StudentId);

                entity.Id = @event.NotificationId;
                entity.SeatId = @event.SeatId;
                entity.SessionId = @event.SessionId;
                entity.CompanyId = @event.CompanyId;
                entity.Label = $"Place de {student.Lastname} {student.Firstname} à valider.";
                entity.AffectedRole = UserRole.Manager;
                entity.ReminderType = NotificationType.SeatToValidate;

                context.SaveChanges();
            }
        }

        public void Handle(AgreementToCreateNotificationSent @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Notifications.FirstOrDefault(a=>a.Id == @event.NotificationId);
                if (entity == null)
                {
                    entity = new NotificationSqlEntity();
                    context.Notifications.Add(entity);
                }

                var company = context.GetEntity<CompanySqlEntity>(@event.CompanyId);

                entity.Id = @event.NotificationId;
                entity.SessionId = @event.SessionId;
                entity.CompanyId = @event.CompanyId;
                entity.ReminderType = NotificationType.AgreementToCreate;
                entity.AffectedRole = UserRole.Operator;
                entity.Label = $"{company.Name} - Convention à créer";

                context.SaveChanges();
            }
        }

        public void Handle(AgreementToSignNotificationSent @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Notifications.FirstOrDefault(a => a.Id == @event.NotificationId);
                if (entity == null)
                {
                    entity = new NotificationSqlEntity();
                    context.Notifications.Add(entity);
                }
                
                var company = context.GetEntity<CompanySqlEntity>(@event.CompanyId);

                entity.Id = @event.NotificationId;
                entity.SessionId = @event.SessionId;
                entity.CompanyId = @event.CompanyId;
                entity.AgreementId = @event.AgreementId;
                entity.ReminderType = NotificationType.AgreementToSign;
                entity.AffectedRole = UserRole.Operator;
                entity.Label = $"{company.Name} - Convention à retourner signée";

                context.SaveChanges();
            }
        }

        public void Handle(NotificationRemoved @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new NotificationSqlEntity(){ Id = @event.NotificationId };
                context.Notifications.Attach(entity);
                context.Notifications.Remove(entity);
                context.SaveChanges();
            }
        }

        public void Handle(NotificationManagerCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.NotificationManagers.FirstOrDefault(a => a.Id == @event.AggregateId);
                if (entity == null)
                {
                    entity = new NotificationManagerSqlEntity();
                    context.NotificationManagers.Add(entity);
                }

                entity.Id = @event.AggregateId;
                entity.SessionId = @event.SessionId;

                context.SaveChanges();
            }
        }
    }
}