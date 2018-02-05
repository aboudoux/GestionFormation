using System;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Seats
{
    public class ValidateSeat : ActionCommand
    {
        public ValidateSeat(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid seatId)
        {            
            var place = GetAggregate<Seat>(seatId);                        
            place.Validate();
            PublishUncommitedEvents(place);
        }
    }
}