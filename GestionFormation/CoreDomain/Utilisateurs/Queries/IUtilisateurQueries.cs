using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Utilisateurs.Queries
{
    public interface IUtilisateurQueries
    {
        IEnumerable<IUtilisateurResult> GetAll();

        IUtilisateurResult GetLogin(string login, string password);

        bool Exists(string login);
    }
}
