using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Conventions.Events
{
    public class ConventionCreated : DomainEvent
    {
        public Guid ContactId { get; }
        public string Convention { get; }

        public ConventionCreated(Guid aggregateId, int sequence, Guid contactId, long numeroConvention) : base(aggregateId, sequence)
        {
            ContactId = contactId;
            Convention = DateTime.Now.Year + " " + numeroConvention + " T";
        }
    }
}