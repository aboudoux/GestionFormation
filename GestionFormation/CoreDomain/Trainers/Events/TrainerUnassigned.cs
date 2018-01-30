using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainers.Events
{
    public class TrainerUnassigned : DomainEvent
    {
        public DateTime SessionStart { get; }
        public int Duration { get; }

        public TrainerUnassigned(Guid aggregateId, int sequence, DateTime sessionStart, int duration) : base(aggregateId, sequence)
        {
            SessionStart = sessionStart;
            Duration = duration;
        }

        protected override string Description => "Formateur désassigné";
    }
}