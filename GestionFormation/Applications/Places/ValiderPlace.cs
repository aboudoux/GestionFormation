using System;
using GestionFormation.CoreDomain.Places;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Places
{
    public class ValiderPlace : ActionCommand
    {
        public ValiderPlace(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid placeId)
        {            
            var place = GetAggregate<Place>(placeId);                        
            place.Validate();
            PublishUncommitedEvents(place);
        }
    }
}