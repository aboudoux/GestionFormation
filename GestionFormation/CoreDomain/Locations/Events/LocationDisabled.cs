using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Locations
{
    public class LocationDisabled : DomainEvent
    {
        public LocationDisabled(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Lieu désactivé";
    }
}