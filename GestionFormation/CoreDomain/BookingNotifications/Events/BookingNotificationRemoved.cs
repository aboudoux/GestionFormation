using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.BookingNotifications.Events
{
    public class BookingNotificationRemoved : DomainEvent
    {
        public BookingNotificationRemoved(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Notification supprimée";
    }
}