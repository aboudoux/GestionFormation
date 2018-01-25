using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Conventions.Events
{
    public class ConventionDisassociated : DomainEvent
    {
        public ConventionDisassociated(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }
    }
}