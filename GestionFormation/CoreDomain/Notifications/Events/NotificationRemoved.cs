using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Notifications.Events
{
    public class NotificationRemoved : DomainEvent, INotificationEvent
    {
        public Guid NotificationId { get; }

        public NotificationRemoved(Guid aggregateId, int sequence, Guid notificationId) : base(aggregateId, sequence)
        {
            NotificationId = notificationId;
        }

        protected override string Description => "Notification supprimée";
    }
}