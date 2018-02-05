using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.BookingNotifications.Events
{
    public abstract class BookingNotificationEvent : DomainEvent
    {
        public Guid SessionId { get; }
        public Guid CompanyId { get; }

        protected BookingNotificationEvent(Guid aggregateId, int sequence, Guid sessionId, Guid companyId) : base(aggregateId, sequence)
        {
            SessionId = sessionId;
            CompanyId = companyId;
        }
    }
}