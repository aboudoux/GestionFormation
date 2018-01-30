using System;
using System.Collections.Generic;
using System.Text;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Seats;
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
            var place = GetAggregate<Seat>(placeId);
            place.Cancel(raison);

            Agreement agreement = null;
            if (place.AssociatedAgreementId.HasValue)
            {
                agreement = GetAggregate<Agreement>(place.AssociatedAgreementId.Value);
                agreement.Revoke();
            }
            
            var session = GetAggregate<Session>(place.SessionId);
            session.ReleasePlace();

            PublishUncommitedEvents(place, agreement, session);
        }
    }
}
