using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Notifications
{
    public abstract class SignaledEvent : DomainEvent
    {
        public Guid SeatId { get; }

        protected SignaledEvent(Guid aggregateId, int sequence, Guid seatId) : base(aggregateId, sequence)
        {
            SeatId = seatId;
        }
    }
}