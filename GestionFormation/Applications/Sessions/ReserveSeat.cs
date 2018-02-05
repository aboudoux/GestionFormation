using System;
using System.Globalization;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Sessions
{
    public class ReserveSeat : ActionCommand
    {
        public ReserveSeat(EventBus eventBus) : base(eventBus)
        {
        }

        public Seat Execute(Guid sessionId, Guid studentId, Guid companyId)
        {
            var session = GetAggregate<Session>(sessionId);
            var seat = session.BookSeat(studentId, companyId);
            PublishUncommitedEvents(session, seat);
            return seat;
        }
    }
}