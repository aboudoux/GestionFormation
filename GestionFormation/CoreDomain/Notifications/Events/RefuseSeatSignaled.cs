using System;

namespace GestionFormation.CoreDomain.Notifications
{
    public class RefuseSeatSignaled : SignaledEvent
    {
        public RefuseSeatSignaled(Guid aggregateId, int sequence, Guid seatId) : base(aggregateId, sequence, seatId)
        {
        }
        protected override string Description => "Refus de place signalée au système de notification";
    }
}