using System;
using System.Collections.Generic;
using System.Text;
using GestionFormation.CoreDomain.Conventions;
using GestionFormation.CoreDomain.Places;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Places
{
    public class AnnulerPlace : ActionCommand
    {
        public AnnulerPlace(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid placeId, string raison)
        {
            var place = GetAggregate<Place>(placeId);
            place.Cancel(raison);

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
