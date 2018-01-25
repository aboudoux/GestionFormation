using System;
using GestionFormation.CoreDomain.Utilisateurs;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Utilisateurs
{
    public class ChangeRole : ActionCommand
    {
        public ChangeRole(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid utilisateurId, UtilisateurRole role)
        {
            var utilisateur = GetAggregate<Utilisateur>(utilisateurId);
            utilisateur.ChangeRole(role);
            PublishUncommitedEvents(utilisateur);
        }
    }
}