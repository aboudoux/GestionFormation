using System;
using GestionFormation.CoreDomain.Conventions;
using GestionFormation.CoreDomain.Places;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Places
{
    public class RefuserPlace : ActionCommand
    {
        public RefuserPlace(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid placeId, string raison)
        {
            var place = GetAggregate<Place>(placeId);
            place.Refuse(raison);

            Convention convention = null;
            if (place.AssociatedConventionId.HasValue)
            {
                convention = GetAggregate<Convention>(place.AssociatedConventionId.Value);
                convention.Revoke();
            }

            var session = GetAggregate<Session>(place.SessionId);
            session.ReleasePlace();

            PublishUncommitedEvents(place, convention, session);
        }
    }
}