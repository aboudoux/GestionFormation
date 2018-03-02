using System;
using GestionFormation.CoreDomain;
using GestionFormation.CoreDomain.Notifications;
using GestionFormation.CoreDomain.Notifications.Queries;
using GestionFormation.Infrastructure.Notifications.Projections;

namespace GestionFormation.Infrastructure.Notifications.Queries
{
    public class NotificationResult : INotificationResult
    {
        public NotificationResult(NotificationSqlEntity entity, string companyName, string trainingName, string studentFirstname, string studentLastname, DateTime sessionDate)
        {
            CompanyName = companyName;
            TrainingName = trainingName;
            StudentName = new FullName(studentLastname, studentFirstname);

            AggregateId = entity.Id;
            Label = entity.Label;
            NotificationType = entity.ReminderType;
            SeatId = entity.SeatId;
            SessionId = entity.SessionId;
            CompanyId = entity.CompanyId;
            AgreementId = entity.AgreementId;
            SessionDate = sessionDate;
        }

        public Guid AggregateId { get; }
        public string Label { get; }
        public NotificationType NotificationType { get; }
        public Guid? SeatId { get; }
        public Guid SessionId { get; }
        public Guid CompanyId { get; }
        public Guid? AgreementId { get; }
        public string CompanyName { get; }
        public string TrainingName { get; }
        public FullName StudentName { get; }
        public DateTime SessionDate { get; }
    }
}