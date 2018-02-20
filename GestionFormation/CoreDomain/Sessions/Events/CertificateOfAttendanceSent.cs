using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions
{
    public class CertificateOfAttendanceSent : DomainEvent
    {
        public Guid StudentId { get; }
        public Guid DocumentId { get; }

        public CertificateOfAttendanceSent(Guid aggregateId, int sequence, Guid studentId, Guid documentId) : base(aggregateId, sequence)
        {
            StudentId = studentId;
            DocumentId = documentId;
        }

        protected override string Description => "Certificat d'assiduité envoyé";
    }
}