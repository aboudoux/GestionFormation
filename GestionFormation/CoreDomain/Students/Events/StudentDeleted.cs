using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Students.Events
{
    public class StudentDeleted : DomainEvent
    {
        public StudentDeleted(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
            
        }

        protected override string Description => "Stagiaire supprimé";
    }
}