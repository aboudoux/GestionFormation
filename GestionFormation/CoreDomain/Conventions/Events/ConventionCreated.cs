using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Conventions.Events
{
    public class ConventionCreated : DomainEvent
    {
        public Guid ContactId { get; }
        public string Convention { get; }
        public TypeConvention TypeConvention { get; }

        public ConventionCreated(Guid aggregateId, int sequence, Guid contactId, long numeroConvention, TypeConvention typeConvention) : base(aggregateId, sequence)
        {
            ContactId = contactId;
            TypeConvention = typeConvention;
            Convention = DateTime.Now.Year + " " + numeroConvention + " T";
        }
    }
}