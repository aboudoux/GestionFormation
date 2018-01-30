using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainings.Events
{
    public class TrainingCreated : DomainEvent
    {
        public string Name { get; }
        public int Seats { get; }

        public TrainingCreated(Guid aggregateId, int sequence, string name, int seats) : base(aggregateId, sequence)
        {
            Name = name;
            Seats = seats;
        }

        protected override string Description => "Formation cr��";
    }
}