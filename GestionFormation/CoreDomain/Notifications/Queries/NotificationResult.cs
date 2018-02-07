using System;
using GestionFormation.CoreDomain.Notifications.Projections;

namespace GestionFormation.CoreDomain.Notifications.Queries
{
    public class NotificationResult : INotificationResult
    {
        public NotificationResult(NotificationSqlEntity entity)
        {
            AggregateId = entity.Id;
            Label = entity.Label;
            NotificationType = entity.ReminderType;
            SeatId = entity.SeatId;
            SessionId = entity.SessionId;
            CompanyId = entity.CompanyId;
            AgreementId = entity.AgreementId;
        }

        public Guid AggregateId { get; }
        public string Label { get; }
        public NotificationType NotificationType { get; }
        public Guid? SeatId { get; }
        public Guid SessionId { get; }
        public Guid CompanyId { get; }
        public Guid? AgreementId { get; }
    }
}