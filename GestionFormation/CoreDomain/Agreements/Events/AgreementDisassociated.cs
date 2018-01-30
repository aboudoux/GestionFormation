using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Agreements.Events
{
    public class AgreementDisassociated : DomainEvent
    {
        public AgreementDisassociated(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Convention désassociée";
    }
}