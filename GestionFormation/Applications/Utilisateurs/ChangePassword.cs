using System;
using GestionFormation.CoreDomain.Users;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Utilisateurs
{
    public class ChangePassword : ActionCommand
    {
        public ChangePassword(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid utilisateurId, string newPassword)
        {
            var utilisateur = GetAggregate<User>(utilisateurId);
            utilisateur.ChangePassword(newPassword);
            PublishUncommitedEvents(utilisateur);
        }
    }
}