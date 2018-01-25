using System;
using GestionFormation.CoreDomain.Utilisateurs;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Utilisateurs
{
    public class UpdateUtilisateur : ActionCommand
    {
        public UpdateUtilisateur(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid utilisateurId, string nom, string prenom, string email, bool isEnabled)
        {
            var utilisateur = GetAggregate<Utilisateur>(utilisateurId);
            utilisateur.Update(nom, prenom, email, isEnabled);
            PublishUncommitedEvents(utilisateur);
        }
    }
}