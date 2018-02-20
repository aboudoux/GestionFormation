using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Events
{
    public class SessionSurveySent : DomainEvent
    {
        public Guid DocumentId { get; }

        public SessionSurveySent(Guid aggregateId, int sequence, Guid documentId) : base(aggregateId, sequence)
        {
            DocumentId = documentId;
        }

        protected override string Description => "Questionnaires envoyés";
    }
}