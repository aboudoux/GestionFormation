using System;
using GestionFormation.CoreDomain.BookingNotifications.Projections;

namespace GestionFormation.CoreDomain.BookingNotifications.Queries
{
    public class NotificationResult : INotificationResult
    {
        public NotificationResult(BookingNotificationSqlEntity entity)
        {
            AggregateId = entity.Id;
            Label = entity.Label;
            BookingNotificationType = entity.ReminderType;
            SeatId = entity.SeatId;
            SessionId = entity.SessionId;
            CompanyId = entity.CompanyId;
            AgreementId = entity.AgreementId;
        }

        public Guid AggregateId { get; }
        public string Label { get; }
        public BookingNotificationType BookingNotificationType { get; }
        public Guid? SeatId { get; }
        public Guid SessionId { get; }
        public Guid CompanyId { get; }
        public Guid? AgreementId { get; }
    }
}