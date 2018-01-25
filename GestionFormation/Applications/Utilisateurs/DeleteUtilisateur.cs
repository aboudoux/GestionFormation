using System;
using GestionFormation.CoreDomain.Utilisateurs;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Utilisateurs
{
    public class DeleteUtilisateur : ActionCommand
    {
        public DeleteUtilisateur(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid utilisateurId)
        {
            var utilisateur = GetAggregate<Utilisateur>(utilisateurId);
            utilisateur.Delete();
            PublishUncommitedEvents(utilisateur);
        }
    }
}