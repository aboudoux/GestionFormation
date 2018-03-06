using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Events
{
    public class SessionSeatBooked : DomainEvent
    {
        public Guid? StudentId { get; }

        public SessionSeatBooked(Guid aggregateId, int sequence, Guid? studentId) : base(aggregateId, sequence)
        {
            StudentId = studentId;
        }

        protected override string Description => "Place reservée";
    }
}