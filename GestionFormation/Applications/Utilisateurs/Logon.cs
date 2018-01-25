using GestionFormation.CoreDomain.Utilisateurs.Queries;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Utilisateurs
{
    public class Logon
    {
        private readonly IUtilisateurQueries _utilisateurQueries;

        public Logon(IUtilisateurQueries utilisateurQueries)
        {
            _utilisateurQueries = utilisateurQueries;
        }

        public LoggedUser Execute(string login, string password)
        {            
            var loggedUser = _utilisateurQueries.GetLogin(login, password);
            if (loggedUser == null)
                throw new BadLoginException();
            return  new LoggedUser(loggedUser.Id, loggedUser.Login, loggedUser.Nom, loggedUser.Prenom, loggedUser.Role);
        }
    }

    public class BadLoginException : DomainException
    {
        public BadLoginException() : base("Utilisateur ou mot de passe incorrect.")
        {
        }
    }
}