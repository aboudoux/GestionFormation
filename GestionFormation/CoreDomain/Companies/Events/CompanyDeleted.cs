using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Companies.Events
{
    public class CompanyDeleted : DomainEvent
    {
        public CompanyDeleted(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Société supprimée";
    }
}