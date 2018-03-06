using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Seats.Events
{
    public class SeatCreated : DomainEvent
    {
        public Guid SessionId { get; }
        public Guid? StudentId { get; }
        public Guid CompanyId { get; }

        public SeatCreated(Guid aggregateId, int sequence, Guid sessionId, Guid? studentId, Guid companyId) : base(aggregateId, sequence)
        {
            SessionId = sessionId;
            StudentId = studentId;
            CompanyId = companyId;
        }

        protected override string Description => "Place créée";
    }
}