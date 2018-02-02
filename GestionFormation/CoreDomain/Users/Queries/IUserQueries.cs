using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Users.Queries
{
    public interface IUserQueries
    {
        IEnumerable<IUserResult> GetAll();

        IUserResult GetLogin(string login, string password);

        bool Exists(string login);
    }
}
