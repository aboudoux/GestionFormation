﻿using System;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Seats
{
    public class CancelSeat : ActionCommand
    {
        public CancelSeat(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid seatId, string reason)
        {
            var seat = GetAggregate<Seat>(seatId);
            seat.Cancel(reason);

            Agreement agreement = null;
            if (seat.AssociatedAgreementId.HasValue)
            {
                agreement = GetAggregate<Agreement>(seat.AssociatedAgreementId.Value);
                agreement.Revoke();
            }
            
            var session = GetAggregate<Session>(seat.SessionId);
            session.ReleaseSeat();

            PublishUncommitedEvents(seat, agreement, session);
        }
    }
}
