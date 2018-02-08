using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Trainers.Queries
{
    public interface ITrainerQueries
    {
        IReadOnlyList<ITrainerResult> GetAll();

        bool Exists(string lastname, string firstname);
    }
}
