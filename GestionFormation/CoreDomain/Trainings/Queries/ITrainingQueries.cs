using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Trainings.Queries
{
    public interface ITrainingQueries
    {
        IReadOnlyList<ITrainingResult> GetAll();

        Guid? GetTrainingId(string trainingName);
    }
}
