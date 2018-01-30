using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Locations.Events
{
    public class LocationAssigned : DomainEvent
    {
        public DateTime SessionStart { get; }
        public int Duration { get; }

        public LocationAssigned(Guid aggregateId, int sequence, DateTime sessionStart, int duration) : base(aggregateId, sequence)
        {
            SessionStart = sessionStart;
            Duration = duration;
        }

        protected override string Description => "Lieu assigné";
    }
}