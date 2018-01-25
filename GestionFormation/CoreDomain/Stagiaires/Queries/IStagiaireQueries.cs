using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Stagiaires.Queries
{
    public interface IStagiaireQueries
    {
        IReadOnlyList<IStagiaireResult> GetAll();
    }
}