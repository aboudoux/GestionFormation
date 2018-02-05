using System;

namespace GestionFormation.CoreDomain.BookingNotifications.Events
{
    public class SeatToValidateSent : BookingNotificationEvent
    {
        public Guid SeatId { get; }

        public SeatToValidateSent(Guid aggregateId, int sequence, Guid sessionId, Guid companyId, Guid seatId) : base(aggregateId, sequence, sessionId, companyId)
        {
            SeatId = seatId;
        }

        protected override string Description => "Place à valider envoyé";
    }
}