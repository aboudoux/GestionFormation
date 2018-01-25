using System.Collections.Generic;
using GestionFormation.CoreDomain.Utilisateurs;

namespace GestionFormation.CoreDomain.Rappels.Queries
{
    public interface IRappelQueries
    {
        IEnumerable<IRappelResult> GetAll(UtilisateurRole role);
    }
}
