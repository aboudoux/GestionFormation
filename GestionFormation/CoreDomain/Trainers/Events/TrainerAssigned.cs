using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainers.Events
{
    public class TrainerAssigned : DomainEvent
    {
        public DateTime SessionStart { get; }
        public int Duration { get; }

        public TrainerAssigned(Guid aggregateId, int sequence, DateTime sessionStart, int duration) : base(aggregateId, sequence)
        {
            SessionStart = sessionStart;
            Duration = duration;
        }

        protected override string Description => "Formateur assigné";
    }
}