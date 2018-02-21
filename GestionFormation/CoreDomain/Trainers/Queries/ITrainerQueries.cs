using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Trainers.Queries
{
    public interface ITrainerQueries
    {
        IReadOnlyList<ITrainerResult> GetAll();

        Guid? GetTrainer(string lastname, string firstname);
    }
}
