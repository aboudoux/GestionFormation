using System;
using GestionFormation.CoreDomain.Locations;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Locations
{
    public class DisableLocation : ActionCommand
    {
        public DisableLocation(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid locationId)
        {
            var location = GetAggregate<Location>(locationId, true);
            location.Disable();
            PublishUncommitedEvents(location);
        }
    }
}