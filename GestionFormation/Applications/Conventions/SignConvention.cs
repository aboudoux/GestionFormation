using System;
using GestionFormation.CoreDomain.Conventions;
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
            var convention = GetAggregate<Convention>(conventionId);
            convention.Sign(documentId);
            PublishUncommitedEvents(convention);
        }
    }
}