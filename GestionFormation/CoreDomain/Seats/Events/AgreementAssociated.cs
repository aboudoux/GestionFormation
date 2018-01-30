using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Seats.Events
{
    public class AgreementAssociated : DomainEvent
    {
        public Guid AgreementId { get; }

        public AgreementAssociated(Guid aggregateId, int sequence, Guid agreementId) : base(aggregateId, sequence)
        {
            AgreementId = agreementId;
        }

        protected override string Description => "Convention associée";
    }
}