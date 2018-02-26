using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainings.Events
{
    public class TrainingCreated : DomainEvent
    {
        public string Name { get; }
        public int Seats { get; }
        public int Color { get; }

        public TrainingCreated(Guid aggregateId, int sequence, string name, int seats, int color) : base(aggregateId, sequence)
        {
            Name = name;
            Seats = seats;
            Color = color;
        }

        protected override string Description => "Formation créé";
    }
}