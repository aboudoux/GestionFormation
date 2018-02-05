using System;
using System.Linq;
using GestionFormation.CoreDomain.BookingNotifications;
using GestionFormation.CoreDomain.BookingNotifications.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.BookingNotifications
{
    public class RemoveBookingNotification : ActionCommand
    {
        private readonly INotificationQueries _notificationQueries;

        public RemoveBookingNotification(EventBus eventBus, INotificationQueries notificationQueries) : base(eventBus)
        {
            _notificationQueries = notificationQueries ?? throw new ArgumentNullException(nameof(notificationQueries));
        }


        public void Execute(Guid notificationId)
        {
            var notif = GetAggregate<BookingNotification>(notificationId);
            notif.Remove();
            PublishUncommitedEvents(notif);
        }

        public void FromAgreement(Guid agreementId)
        {
            var notification = _notificationQueries.GetFromAgreement(agreementId).First();
            Execute(notification.AggregateId);
        }

        public void FromSeat(Guid seatId)
        {
            var notification = _notificationQueries.GetFromSeat(seatId).First();
            Execute(notification.AggregateId);
        }
    }
}