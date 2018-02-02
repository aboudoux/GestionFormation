using System;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Agreements
{
    public class SignAgreement : ActionCommand
    {
        public SignAgreement(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid conventionId, Guid documentId)
        {
            var agreement = GetAggregate<Agreement>(conventionId);
            agreement.Sign(documentId);
            PublishUncommitedEvents(agreement);
        }
    }
}