using System;
using GestionFormation.CoreDomain.Utilisateurs;
using GestionFormation.CoreDomain.Utilisateurs.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Utilisateurs
{
    public class CreateUtilisateur : ActionCommand
    {
        private readonly IUtilisateurQueries _utilisateurQueries;

        public CreateUtilisateur(EventBus eventBus, IUtilisateurQueries utilisateurQueries) : base(eventBus)
        {
            _utilisateurQueries = utilisateurQueries ?? throw new ArgumentNullException(nameof(utilisateurQueries));
        }

        public void Execute(string login, string password, string nom, string prenom, string email, UtilisateurRole role)
        {
            if(_utilisateurQueries.Exists(login))
                throw new UtilisateurAlreadyExistsException();

            var utilisateur = Utilisateur.Create(login, password, nom, prenom, email, role);
            PublishUncommitedEvents(utilisateur);
        }
    }
}
