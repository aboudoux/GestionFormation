using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Seats
{
    public class CertificatOfAttendanceSent : DomainEvent
    {
        public Guid DocumentId { get; }

        public CertificatOfAttendanceSent(Guid aggregateId, int sequence, Guid documentId) : base(aggregateId, sequence)
        {
            DocumentId = documentId;
        }

        protected override string Description => "Certificat d'assiduité envoyé";
    }
}