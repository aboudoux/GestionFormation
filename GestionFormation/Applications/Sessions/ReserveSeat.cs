using System;
using GestionFormation.CoreDomain.Notifications;
using GestionFormation.CoreDomain.Notifications.Queries;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Sessions
{
    public class ReserveSeat : ActionCommand
    {
        private readonly INotificationQueries _notificationManagerQueries;

        public ReserveSeat(EventBus eventBus, INotificationQueries notificationManagerQueries) : base(eventBus)
        {
            _notificationManagerQueries = notificationManagerQueries ?? throw new ArgumentNullException(nameof(notificationManagerQueries));
        }

        public Seat Execute(Guid sessionId, Guid studentId, Guid companyId, bool sendNotification)
        {
            var session = GetAggregate<Session>(sessionId);
            var seat = session.BookSeat(studentId, companyId);

            var managerId = _notificationManagerQueries.GetNotificationManagerId(sessionId);
            var manager = GetAggregate<NotificationManager>(managerId);
            
            manager.SignalSeatCreated(seat.AggregateId, companyId, sendNotification);

            PublishUncommitedEvents(session, seat, manager);
            return seat;
        }
    }
}