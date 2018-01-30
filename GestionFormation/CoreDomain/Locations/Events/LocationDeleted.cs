using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Locations.Events
{
    public class LocationDeleted : DomainEvent
    {
        public LocationDeleted(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Lieu supprimé";
    }
}