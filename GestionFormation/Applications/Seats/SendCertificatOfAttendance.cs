using System;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Seats
{
    public class SendCertificatOfAttendance : ActionCommand
    {
        public SendCertificatOfAttendance(EventBus eventBus) : base(eventBus)
        {
        }

        public void Execute(Guid seatId, Guid documentId)
        {
            var seat = GetAggregate<Seat>(seatId);
            seat.SendCertificatOfAttendance(documentId);
            PublishUncommitedEvents(seat);
        }
    }
}