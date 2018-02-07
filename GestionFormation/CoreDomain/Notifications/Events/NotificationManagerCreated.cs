using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Notifications.Events
{
    public class NotificationManagerCreated : DomainEvent
    {
        public Guid SessionId { get; }

        public NotificationManagerCreated(Guid aggregateId, int sequence, Guid sessionId) : base(aggregateId, sequence)
        {
            SessionId = sessionId;
        }

        protected override string Description => "Notification de session créée";
    }
}