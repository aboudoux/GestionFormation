using System;
using GestionFormation.CoreDomain.Trainings.Projections;

namespace GestionFormation.CoreDomain.Trainings.Queries
{
    public class TrainingResult : ITrainingResult
    {
        public TrainingResult(TrainingSqlEntity entity)
        {
            Id = entity.TrainingId;
            Name = entity.Name;
            Seats = entity.Seats;
        }
        public Guid Id { get; }
        public string Name { get; }
        public int Seats { get; }
    }
}