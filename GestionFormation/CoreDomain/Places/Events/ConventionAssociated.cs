using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Places.Events
{
    public class ConventionAssociated : DomainEvent
    {
        public Guid ConventionId { get; }

        public ConventionAssociated(Guid aggregateId, int sequence, Guid conventionId) : base(aggregateId, sequence)
        {
            ConventionId = conventionId;
        }

        protected override string Description => "Convention associée";
    }
}