using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Conventions.Events
{
    public class ConventionRevoked : DomainEvent
    {
        public ConventionRevoked(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Convention révoquée";
    }
}