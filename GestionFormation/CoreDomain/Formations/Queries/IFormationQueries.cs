using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Formations.Queries
{
    public interface IFormationQueries
    {
        IReadOnlyList<IFormationResult> GetAll();

        Guid? GetFormation(string formationName);
    }
}
