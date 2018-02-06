using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.BookingNotifications.Events
{
    public class BookingNotificationRemoved : DomainEvent
    {
        public Guid NotificationId { get; }

        public BookingNotificationRemoved(Guid aggregateId, int sequence, Guid notificationId) : base(aggregateId, sequence)
        {
            NotificationId = notificationId;
        }

        protected override string Description => "Notification supprimée";
    }
}