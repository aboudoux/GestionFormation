using System;
using GestionFormation.CoreDomain.Locations;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Locations
{
    public class DeleteLocation : ActionCommand
    {
        public DeleteLocation(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid locationId)
        {
            var location = GetAggregate<Location>(locationId);
            location.Delete();
            PublishUncommitedEvents(location);
        }
    }
}