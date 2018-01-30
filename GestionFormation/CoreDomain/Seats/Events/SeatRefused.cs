using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Seats.Events
{
    public class SeatRefused : DomainEvent
    {
        public string Reason { get; }

        public SeatRefused(Guid aggregateId, int sequence, string reason) : base(aggregateId, sequence)
        {
            Reason = reason;
        }

        protected override string Description => "Place refusée";
    }
}