using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainers.Events
{
    public class TrainerReassigned : DomainEvent
    {
        public DateTime OldSessionStart { get; }
        public int OldDuration { get; }
        public DateTime NewSessionStart { get; }
        public int NewDuration { get; }

        public TrainerReassigned(Guid aggregateId, int sequence, DateTime oldSessionStart, int oldDuration, DateTime newSessionStart, int newDuration) : base(aggregateId, sequence)
        {
            OldSessionStart = oldSessionStart;
            OldDuration = oldDuration;
            NewSessionStart = newSessionStart;
            NewDuration = newDuration;
        }

        protected override string Description => "Formateur réassigné";
    }
}