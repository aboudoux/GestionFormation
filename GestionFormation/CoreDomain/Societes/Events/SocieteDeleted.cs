using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Societes
{
    public class SocieteDeleted : DomainEvent
    {
        public SocieteDeleted(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Société supprimée";
    }
}