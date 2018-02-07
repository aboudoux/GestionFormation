using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Notifications.Events
{
    public abstract class NotificationEvent : DomainEvent
    {
        [IgnoreEquality]
        public Guid NotificationId { get; }
        public Guid SessionId { get; }
        public Guid CompanyId { get; }

        protected NotificationEvent(Guid aggregateId, int sequence, Guid sessionId, Guid companyId, Guid notificationId) : base(aggregateId, sequence)
        {
            NotificationId = notificationId;
            SessionId = sessionId;
            CompanyId = companyId;
        }
    }
}