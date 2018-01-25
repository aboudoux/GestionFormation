using System;
using GestionFormation.CoreDomain.Places;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Sessions
{
    public class ReleasePlace : ActionCommand
    {
        public ReleasePlace(EventBus eventBus) : base(eventBus)
        {            
        }

        public void Execute(Guid sessionId, Guid placeId, string reason)
        {
            var session = GetAggregate<Session>(sessionId);
            var place = GetAggregate<Place>(placeId);

            session.ReleasePlace();
            place.Cancel(reason);

            PublishUncommitedEvents(session, place);
        }
    }
}