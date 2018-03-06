using System;

namespace GestionFormation.CoreDomain.Notifications
{
    public class CancelSeatSignaled : SignaledEvent
    {
        public CancelSeatSignaled(Guid aggregateId, int sequence, Guid seatId) : base(aggregateId, sequence, seatId)
        {
        }

        protected override string Description => "Annulation de place signalée au système de notification";
    }
}