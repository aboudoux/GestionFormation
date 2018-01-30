using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Events
{
    public class SessionSeatBooked : DomainEvent
    {
        public SessionSeatBooked(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Place reservée";
    }
}