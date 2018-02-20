using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Events
{
    public class SessionSeatReleased : DomainEvent
    {
        public Guid StudentId { get; }

        public SessionSeatReleased(Guid aggregateId, int sequence, Guid studentId) : base(aggregateId, sequence)
        {
            StudentId = studentId;
        }

        protected override string Description => "Place libérée";
    }
}