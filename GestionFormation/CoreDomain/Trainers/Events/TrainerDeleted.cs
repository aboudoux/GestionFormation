using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainers.Events
{
    public class TrainerDeleted : DomainEvent
    {
        public TrainerDeleted(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Formateur supprimé";
    }
}