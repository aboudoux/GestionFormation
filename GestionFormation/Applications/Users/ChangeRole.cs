using System;
using GestionFormation.CoreDomain.Users;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Users
{
    public class ChangeRole : ActionCommand
    {
        public ChangeRole(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid utilisateurId, UserRole role)
        {
            var utilisateur = GetAggregate<User>(utilisateurId);
            utilisateur.ChangeRole(role);
            PublishUncommitedEvents(utilisateur);
        }
    }
}