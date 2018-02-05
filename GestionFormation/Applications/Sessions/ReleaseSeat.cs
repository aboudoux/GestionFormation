using System;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Sessions
{
    public class ReleaseSeat : ActionCommand
    {
        public ReleaseSeat(EventBus eventBus) : base(eventBus)
        {            
        }

        public void Execute(Guid sessionId, Guid placeId, string reason)
        {
            var session = GetAggregate<Session>(sessionId);
            var place = GetAggregate<Seat>(placeId);

            session.ReleasePlace();
            place.Cancel(reason);

            PublishUncommitedEvents(session, place);
        }
    }
}