using System;
using GestionFormation.CoreDomain.Seats;
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
            var place = GetAggregate<Seat>(placeId);                        
            place.Validate();
            PublishUncommitedEvents(place);
        }
    }
}