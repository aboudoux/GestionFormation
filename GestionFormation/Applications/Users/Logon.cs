using GestionFormation.CoreDomain.Users.Queries;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Users
{
    public class Logon
    {
        private readonly IUserQueries _userQueries;

        public Logon(IUserQueries userQueries)
        {
            _userQueries = userQueries;
        }

        public LoggedUser Execute(string login, string password)
        {            
            var loggedUser = _userQueries.GetLogin(login, password);
            if (loggedUser == null)
                throw new BadLoginException();
            return  new LoggedUser(loggedUser.Id, loggedUser.Login, loggedUser.Lastname, loggedUser.Firsname, loggedUser.Role, loggedUser.Signature);
        }
    }

    public class BadLoginException : DomainException
    {
        public BadLoginException() : base("Utilisateur ou mot de passe incorrect.")
        {
        }
    }
}