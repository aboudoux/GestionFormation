using System;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Conventions
{
    public class SignConvention : ActionCommand
    {
        public SignConvention(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid conventionId, Guid documentId)
        {
            var convention = GetAggregate<Agreement>(conventionId);
            convention.Sign(documentId);
            PublishUncommitedEvents(convention);
        }
    }
}