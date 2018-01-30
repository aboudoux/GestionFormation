using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Agreements.Events
{
    public class AgreementRevoked : DomainEvent
    {
        public AgreementRevoked(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Convention révoquée";
    }
}