using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Conventions
{
    public class ConventionSigned : DomainEvent
    {
        public Guid DocumentId { get; }

        public ConventionSigned(Guid aggregateId, int sequence, Guid documentId) : base(aggregateId, sequence)
        {
            DocumentId = documentId;
        }
    }
}