using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Events
{
    public class SessionTimesheetSent : DomainEvent
    {
        public Guid DocumentId { get; }

        public SessionTimesheetSent(Guid aggregateId, int sequence, Guid documentId) : base(aggregateId, sequence)
        {
            DocumentId = documentId;
        }

        protected override string Description => "Feuille de présence envoyée";
    }
}