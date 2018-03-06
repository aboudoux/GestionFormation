using System;
using GestionFormation.CoreDomain.Notifications;
using GestionFormation.CoreDomain.Notifications.Queries;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Sessions
{
    public class ReleaseSeat : ActionCommand
    {
        private readonly INotificationQueries _notificationManagerQueries;

        public ReleaseSeat(EventBus eventBus, INotificationQueries notificationManagerQueries) : base(eventBus)
        {
            _notificationManagerQueries = notificationManagerQueries ?? throw new ArgumentNullException(nameof(notificationManagerQueries));
        }

        public void Execute(Guid sessionId, Guid seatId, string reason)
        {
            var session = GetAggregate<Session>(sessionId);
            var seat = GetAggregate<Seat>(seatId);

            if(seat.StudentId.HasValue)
                session.ReleaseSeat(seat.StudentId.Value);
            seat.Cancel(reason);

            var managerId = _notificationManagerQueries.GetNotificationManagerId(sessionId);
            var manager = GetAggregate<NotificationManager>(managerId);
            manager.SignalSeatCanceled(seat.AggregateId, seat.CompanyId);

            PublishUncommitedEvents(session, seat);
        }
    }
}