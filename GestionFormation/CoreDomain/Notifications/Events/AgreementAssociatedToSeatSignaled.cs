using System;

namespace GestionFormation.CoreDomain.Notifications
{
    public class AgreementAssociatedToSeatSignaled : SignaledEvent
    {
        public Guid AgreementId { get; }

        public AgreementAssociatedToSeatSignaled(Guid aggregateId, int sequence, Guid seatId, Guid agreementId) : base(aggregateId, sequence, seatId)
        {
            AgreementId = agreementId;
        }

        protected override string Description => "Convention associée à une place signalée au système de notification";
    }
}