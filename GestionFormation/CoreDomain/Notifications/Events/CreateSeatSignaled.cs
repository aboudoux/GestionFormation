using System;

namespace GestionFormation.CoreDomain.Notifications
{
    public class CreateSeatSignaled : SignaledEvent
    {
        public Guid CompanyId { get; }

        public CreateSeatSignaled(Guid aggregateId, int sequence, Guid seatId, Guid companyId) : base(aggregateId, sequence, seatId)
        {
            CompanyId = companyId;
        }

        protected override string Description => "Création de place signalée au système de notification";
    }
}