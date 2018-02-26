using System;

namespace GestionFormation.CoreDomain.Trainings.Queries
{
    public interface ITrainingResult
    {
        Guid Id { get; }
        string Name { get; }
        int Seats { get; }
        int Color { get; }
    }
}