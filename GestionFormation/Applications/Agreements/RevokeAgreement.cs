using System;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Agreements
{
    public class RevokeAgreement : ActionCommand
    {
        public RevokeAgreement(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid conventionId)
        {
            var agreement = GetAggregate<Agreement>(conventionId);
            agreement.Revoke();
            PublishUncommitedEvents(agreement);
        }
    }
}