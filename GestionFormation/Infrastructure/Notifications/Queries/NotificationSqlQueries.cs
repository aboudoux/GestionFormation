using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Notifications;
using GestionFormation.CoreDomain.Notifications.Queries;
using GestionFormation.CoreDomain.Users;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure.Notifications.Queries
{
    public class NotificationQueries : INotificationQueries, IRuntimeDependency
    {
        public IEnumerable<INotificationResult> GetAll(UserRole role)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var allAgreementNotification = from n in context.Notifications
                    join company in context.Companies on n.CompanyId equals company.CompanyId
                    join session in context.Sessions on n.SessionId equals session.SessionId
                    join training in context.Trainings on session.TrainingId equals training.TrainingId
                    where n.ReminderType == NotificationType.AgreementToCreate ||
                          n.ReminderType == NotificationType.AgreementToSign
                    select new {Notification = n, CompanyName = company.Name, TrainingName = training.Name, Date = session.SessionStart};

                var allValidationNotification = from n in context.Notifications
                    join company in context.Companies on n.CompanyId equals company.CompanyId
                    join session in context.Sessions on n.SessionId equals session.SessionId
                    join training in context.Trainings on session.TrainingId equals training.TrainingId
                    join seat in  context.Seats on n.SeatId equals seat.SeatId
                    join student in context.Students on seat.StudentId equals student.StudentId 
                    where n.ReminderType == NotificationType.SeatToValidate
                    select new { Notification = n, CompanyName = company.Name, TrainingName = training.Name, StudentFirstname = student.Firstname, StudentLastname = student.Lastname, Date = session.SessionStart };

                var result = new List<INotificationResult>();

                if (role == UserRole.Admin) { 
                    result.AddRange(allAgreementNotification.ToList().Select(a=>new NotificationResult(a.Notification, a.CompanyName, a.TrainingName, string.Empty, string.Empty, a.Date)));
                    result.AddRange(allValidationNotification.ToList().Select(a=>new NotificationResult(a.Notification, a.CompanyName, a.TrainingName, a.StudentFirstname, a.StudentLastname, a.Date)));
                    return result;
                }

                result.AddRange(allAgreementNotification.Where(a => a.Notification.AffectedRole == role).ToList().Select(a => new NotificationResult(a.Notification, a.CompanyName, a.TrainingName, string.Empty, string.Empty, a.Date)));
                result.AddRange(allValidationNotification.Where(a => a.Notification.AffectedRole == role).ToList().Select(a => new NotificationResult(a.Notification, a.CompanyName, a.TrainingName, a.StudentFirstname, a.StudentLastname, a.Date)));
                return result;

                /*if ( role == UserRole.Admin)
                    return context.Notifications.ToList().Select(a => new NotificationResult(a));                
                return context.Notifications.Where(a => a.AffectedRole == role).ToList().Select(a => new NotificationResult(a));*/
            }
        }

        public Guid GetNotificationManagerId(Guid sessionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.NotificationManagers.First(a => a.SessionId == sessionId).Id;
            }
        }

        public Guid GetNotificationManagerIdFromAgreement(Guid agreementId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return (from seat in context.Seats
                    where seat.AssociatedAgreementId == agreementId
                    join n in context.NotificationManagers on seat.SessionId equals n.SessionId
                    select n.Id).First();
            }
        }
    }
}