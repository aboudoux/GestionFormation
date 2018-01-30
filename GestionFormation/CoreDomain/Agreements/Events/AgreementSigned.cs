using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Agreements.Events
{
    public class AgreementSigned : DomainEvent
    {
        public Guid DocumentId { get; }

        public AgreementSigned(Guid aggregateId, int sequence, Guid documentId) : base(aggregateId, sequence)
        {
            DocumentId = documentId;
        }

        protected override string Description => "Convention signée";
    }
}