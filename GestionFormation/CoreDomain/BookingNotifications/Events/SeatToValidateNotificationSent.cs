using System;

namespace GestionFormation.CoreDomain.BookingNotifications.Events
{
    public class SeatToValidateNotificationSent : BookingNotificationEvent
    {
        public Guid SeatId { get; }

        public SeatToValidateNotificationSent(Guid aggregateId, int sequence, Guid sessionId, Guid companyId, Guid seatId) : base(aggregateId, sequence, sessionId, companyId)
        {
            SeatId = seatId;
        }

        protected override string Description => "Place à valider envoyé";
    }
}