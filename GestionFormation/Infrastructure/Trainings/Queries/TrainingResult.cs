using System;
using GestionFormation.CoreDomain.Trainings.Queries;
using GestionFormation.Infrastructure.Trainings.Projections;

namespace GestionFormation.Infrastructure.Trainings.Queries
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