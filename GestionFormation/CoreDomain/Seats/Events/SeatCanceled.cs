using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Seats.Events
{
    public class SeatCanceled : DomainEvent
    {
        public string Reason { get; }
        public SeatCanceled(Guid aggregateId, int sequence, string reason) : base(aggregateId, sequence)
        {
            Reason = reason;
        }

        protected override string Description => "Place annulée";
    }
}