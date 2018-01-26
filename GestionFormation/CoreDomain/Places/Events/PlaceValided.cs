using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Places.Events
{
    public class PlaceValided : DomainEvent
    {
        public PlaceValided(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Place validée";
    }    
}