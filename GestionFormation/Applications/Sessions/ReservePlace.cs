using System;
using System.Globalization;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Sessions
{
    public class ReservePlace : ActionCommand
    {
        public ReservePlace(EventBus eventBus) : base(eventBus)
        {
        }

        public Seat Execute(Guid sessionId, Guid stagiaireId, Guid societeId)
        {
            var session = GetAggregate<Session>(sessionId);
            var place = session.BookSeat(stagiaireId, societeId);
            PublishUncommitedEvents(session, place);
            return place;
        }
    }
}