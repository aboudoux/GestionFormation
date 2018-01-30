using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Locations.Events
{
    public class LocationReassigned : DomainEvent
    {
        public DateTime OldSessionStart { get; }
        public int OldDuration { get; }
        public DateTime NewSessionStart { get; }
        public int NewDuration { get; }

        public LocationReassigned(Guid aggregateId, int sequence, DateTime oldSessionStart, int oldDuration, DateTime newSessionStart, int newDuration) : base(aggregateId, sequence)
        {
            OldSessionStart = oldSessionStart;
            OldDuration = oldDuration;
            NewSessionStart = newSessionStart;
            NewDuration = newDuration;
        }

        protected override string Description => "Lieu réassigné";
    }
}