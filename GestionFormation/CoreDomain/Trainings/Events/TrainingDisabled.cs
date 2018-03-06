using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainings.Events
{
    public class TrainingDisabled : DomainEvent
    {
        public TrainingDisabled(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Formation désactivée";
    }
}