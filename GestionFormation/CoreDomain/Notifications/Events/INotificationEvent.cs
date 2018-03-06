using System;

namespace GestionFormation.CoreDomain.Notifications.Events
{
    public interface INotificationEvent
    {
        Guid NotificationId { get; }
    }
}