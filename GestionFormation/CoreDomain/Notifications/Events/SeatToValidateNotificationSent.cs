using System;

namespace GestionFormation.CoreDomain.Notifications.Events
{
    public class SeatToValidateNotificationSent : NotificationEvent
    {
        public Guid SeatId { get; }

        public SeatToValidateNotificationSent(Guid aggregateId, int sequence, Guid sessionId, Guid companyId, Guid seatId, Guid notificationId) : base(aggregateId, sequence, sessionId, companyId, notificationId)
        {
            SeatId = seatId;
        }

        protected override string Description => "Place à valider envoyé";
    }
}