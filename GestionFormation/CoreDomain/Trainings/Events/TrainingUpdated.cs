using System;
using System.Drawing;

namespace GestionFormation.CoreDomain.Trainings.Events
{
    public class TrainingUpdated : TrainingCreated
    {
        public TrainingUpdated(Guid aggregateId, int sequence, string name, int seats, int color) : base(aggregateId, sequence, name, seats, color)
        {
        }

        protected override string Description => "Formation modifiée";
    }
}