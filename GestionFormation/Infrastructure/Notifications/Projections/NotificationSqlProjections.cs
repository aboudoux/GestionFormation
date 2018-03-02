using System.Linq;
using GestionFormation.CoreDomain.Notifications;
using GestionFormation.CoreDomain.Notifications.Events;
using GestionFormation.CoreDomain.Users;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure.Notifications.Projections
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
              
                entity.Id = @event.NotificationId;
                entity.SeatId = @event.SeatId;
                entity.SessionId = @event.SessionId;
                entity.CompanyId = @event.CompanyId;
                entity.Label = "Place(s) à valider.";
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

                entity.Id = @event.NotificationId;
                entity.SessionId = @event.SessionId;
                entity.CompanyId = @event.CompanyId;
                entity.ReminderType = NotificationType.AgreementToCreate;
                entity.AffectedRole = UserRole.Operator;
                entity.Label = "Convention(s) à créer";

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
                
                entity.Id = @event.NotificationId;
                entity.SessionId = @event.SessionId;
                entity.CompanyId = @event.CompanyId;
                entity.AgreementId = @event.AgreementId;
                entity.ReminderType = NotificationType.AgreementToSign;
                entity.AffectedRole = UserRole.Operator;
                entity.Label = "Convention(s) à retourner signée(s)";

                context.SaveChanges();
            }
        }

        public void Handle(NotificationRemoved @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                context.Database.ExecuteSqlCommand($"DELETE FROM dbo.Notification WHERE Id = '{@event.NotificationId}'");
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