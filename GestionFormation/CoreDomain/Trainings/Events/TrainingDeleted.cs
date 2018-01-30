using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainings.Events
{
    public class TrainingDeleted : DomainEvent
    {
        public TrainingDeleted(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Formation supprimé";
    }
}