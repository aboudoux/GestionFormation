using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainings.Events
{
    public class TrainingUpdated : DomainEvent
    {
        public string Name { get; }
        public int Seats { get; }

        public TrainingUpdated(Guid aggregateId, int sequence, string name, int seats) : base(aggregateId, sequence)
        {
            Name = name;
            Seats = seats;
        }

        protected override string Description => "Formation modifiée";
    }
}