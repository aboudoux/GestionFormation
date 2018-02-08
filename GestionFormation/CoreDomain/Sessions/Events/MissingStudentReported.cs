using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Students.Events
{
    public class MissingStudentReported : DomainEvent
    {
        public Guid StudentId { get; }

        public MissingStudentReported(Guid aggregateId, int sequence, Guid studentId) : base(aggregateId, sequence)
        {
            StudentId = studentId;
        }

        protected override string Description => "Absence de stagiaire signalé";
    }
}