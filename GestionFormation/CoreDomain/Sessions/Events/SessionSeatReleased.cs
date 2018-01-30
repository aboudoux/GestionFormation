using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Events
{
    public class SessionSeatReleased : DomainEvent
    {
        public SessionSeatReleased(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Place libérée";
    }
}