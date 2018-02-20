using System;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Seats
{
    public class ReportMissingStudent : ActionCommand
    {
        public ReportMissingStudent(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid seatId)
        {
            var seat = GetAggregate<Seat>(seatId);
            seat.ReportMissingStudent();
            PublishUncommitedEvents(seat);
        }
    }
}