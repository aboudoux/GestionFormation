using System;
using GestionFormation.CoreDomain.Notifications;
using GestionFormation.CoreDomain.Notifications.Queries;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Seats
{
    public class ValidateSeat : ActionCommand
    {
        private readonly INotificationQueries _notificationQueries;


        public ValidateSeat(EventBus eventBus, INotificationQueries notificationQueries) : base(eventBus)
        {
            _notificationQueries = notificationQueries ?? throw new ArgumentNullException(nameof(notificationQueries));
        }

        public void Execute(Guid seatId)
        {            
            var seat = GetAggregate<Seat>(seatId);                              
            seat.Validate();

            var managerId = _notificationQueries.GetNotificationManagerId(seat.SessionId);
            var manager = GetAggregate<NotificationManager>(managerId);

            manager.SignalSeatValidated(seat.AggregateId, seat.CompanyId);

            PublishUncommitedEvents(seat, manager);
        }
    }
}