using System;
using GestionFormation.CoreDomain.Users;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Utilisateurs
{
    public class DeleteUser : ActionCommand
    {
        public DeleteUser(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid userId)
        {
            var user = GetAggregate<User>(userId);
            user.Delete();
            PublishUncommitedEvents(user);
        }
    }
}