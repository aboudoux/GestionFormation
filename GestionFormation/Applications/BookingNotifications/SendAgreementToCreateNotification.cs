using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.BookingNotifications;
using GestionFormation.CoreDomain.BookingNotifications.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.BookingNotifications
{
    public class SendAgreementToCreateNotification : ActionCommand
    {
        private readonly INotificationQueries _notificationQueries;

        public SendAgreementToCreateNotification(EventBus eventBus, INotificationQueries notificationQueries) : base(eventBus)
        {
            _notificationQueries = notificationQueries ?? throw new ArgumentNullException(nameof(notificationQueries));
        }

        public void Execute(Guid sessionId, Guid companyId)
        {
            var aggregates = new List<AggregateRoot>();

            var notifications = _notificationQueries.GetAll(sessionId, companyId);

            if (notifications.Any())
            {
                var firstRecord = true;
                foreach (var notification in notifications)
                {
                    var aggregate = GetAggregate<BookingNotification>(notification.AggregateId);
                    if (firstRecord)
                    {
                        aggregate.ChangeToAgreementToCreate();
                        firstRecord = false;
                    }
                    else
                        aggregate.Remove();

                    aggregates.Add(aggregate);
                }
            }
            else
            {
                aggregates.Add(BookingNotification.SendAgreementToCreate(sessionId, companyId));
            }

            PublishUncommitedEvents(aggregates.ToArray());
        }
    }
}