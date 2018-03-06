using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Seats
{
    public class SeatStudentUpdated : DomainEvent
    {
        public Guid? NewStudentId { get; }

        public SeatStudentUpdated(Guid aggregateId, int sequence, Guid? newStudentId) : base(aggregateId, sequence)
        {
            NewStudentId = newStudentId;
        }

        protected override string Description => "Stagiaire à former modifié";
    }
}