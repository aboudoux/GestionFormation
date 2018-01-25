using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Places.Events
{
    public class PlaceCanceled : DomainEvent
    {
        public string Reason { get; }
        public PlaceCanceled(Guid aggregateId, int sequence, string reason) : base(aggregateId, sequence)
        {
            Reason = reason;
        }
    }
}