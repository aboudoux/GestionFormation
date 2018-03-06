using System;
using GestionFormation.CoreDomain.Users;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Users
{
    public class UpdateUser : ActionCommand
    {
        public UpdateUser(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid userId, string lastname, string firstname, string email, bool isEnabled, string signature)
        {
            var user = GetAggregate<User>(userId);
            user.Update(lastname, firstname, email, isEnabled, signature);
            PublishUncommitedEvents(user);
        }
    }
}