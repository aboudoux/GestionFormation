using System;
using System.Runtime.Serialization;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.BookingNotifications.Events
{
    public abstract class BookingNotificationEvent : DomainEvent
    {
        [IgnoreEquality]
        public Guid NotificationId { get; }
        public Guid SessionId { get; }
        public Guid CompanyId { get; }

        protected BookingNotificationEvent(Guid aggregateId, int sequence, Guid sessionId, Guid companyId) : base(aggregateId, sequence)
        {
            NotificationId = Guid.NewGuid();
            SessionId = sessionId;
            CompanyId = companyId;
        }
    }
}