using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Locations.Events
{
    public class LocationCreated : DomainEvent
    {
        public string Name { get; }
        public string Address { get; }
        public int Seats { get; }

        public LocationCreated(Guid aggregateId, int sequence, string name, string address, int seats) : base(aggregateId, sequence)
        {
            Name = name;
            Address = address;
            Seats = seats;
        }

        protected override string Description => "Lieu créé";
    }
}