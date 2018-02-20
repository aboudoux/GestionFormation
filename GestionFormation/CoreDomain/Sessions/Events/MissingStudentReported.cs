using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Events
{
    public class MissingStudentReported : DomainEvent
    {

        public MissingStudentReported(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Absence de stagiaire signalé";
    }
}