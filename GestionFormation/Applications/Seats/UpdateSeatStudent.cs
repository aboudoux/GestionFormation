using System;
using GestionFormation.CoreDomain.Notifications;
using GestionFormation.CoreDomain.Notifications.Queries;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Seats
{
    public class UpdateSeatStudent : ActionCommand
    {
        private readonly INotificationQueries _notificationQueries;

        public UpdateSeatStudent(EventBus eventBus, INotificationQueries notificationQueries) : base(eventBus)
        {
            _notificationQueries = notificationQueries ?? throw new ArgumentNullException(nameof(notificationQueries));
        }

        public void Execute(Guid seatId, Guid? newStudentId)
        {
            GuidAssert.AreNotEmpty(seatId);

            var seat = GetAggregate<Seat>(seatId, true);
            seat.UpdateStudent(newStudentId);

            var notificationManagerId  = _notificationQueries.GetNotificationManagerId(seat.SessionId);
            var notificationManager = GetAggregate<NotificationManager>(notificationManagerId, true);
            notificationManager.SignalSeatRedefined(seatId, seat.CompanyId, newStudentId);

            PublishUncommitedEvents(seat, notificationManager);
        }
    }
}