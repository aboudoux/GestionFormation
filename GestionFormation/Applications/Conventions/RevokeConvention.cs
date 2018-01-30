using System;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Conventions
{
    public class RevokeConvention : ActionCommand
    {
        public RevokeConvention(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid conventionId)
        {
            var convention = GetAggregate<Agreement>(conventionId);
            convention.Revoke();
            PublishUncommitedEvents(convention);
        }
    }
}