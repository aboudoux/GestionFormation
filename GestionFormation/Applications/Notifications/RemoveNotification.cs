using System;
using System.Collections.Generic;
using System.Text;
using GestionFormation.CoreDomain.Notifications;
using GestionFormation.CoreDomain.Notifications.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Notifications
{
    public class RemoveNotification : ActionCommand
    {
        private readonly INotificationQueries _managerQueries;

        public RemoveNotification(EventBus eventBus, INotificationQueries managerQueries) : base(eventBus)
        {
            _managerQueries = managerQueries ?? throw new ArgumentNullException(nameof(managerQueries));
        }

        public void Execute(Guid sessionId, Guid notificationId)
        {
            var managerId = _managerQueries.GetNotificationManagerId(sessionId);
            var manager = GetAggregate<NotificationManager>(managerId);
            manager.Remove(notificationId);

            PublishUncommitedEvents(manager);
        }
    }
}
