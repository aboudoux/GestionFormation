using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Seats.Events
{
    public class SeatValided : DomainEvent
    {
        public SeatValided(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Place validée";
    }    
}