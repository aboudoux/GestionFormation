using System;

namespace GestionFormation.CoreDomain.Notifications
{
    public class ValidateSeatSignaled : SignaledEvent
    {
        public ValidateSeatSignaled(Guid aggregateId, int sequence, Guid seatId) : base(aggregateId, sequence, seatId)
        {
        }

        protected override string Description => "Validation de place signalé au système de notification";
    }
}