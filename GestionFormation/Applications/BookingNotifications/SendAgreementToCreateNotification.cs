using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.BookingNotifications;
using GestionFormation.CoreDomain.BookingNotifications.Queries;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Seats.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.BookingNotifications
{
    public class SendAgreementToCreateNotification : ActionCommand
    {
        private readonly INotificationQueries _notificationQueries;
        private readonly ISeatQueries _seatQueries;

        public SendAgreementToCreateNotification(EventBus eventBus, INotificationQueries notificationQueries, ISeatQueries seatQueries) : base(eventBus)
        {
            _notificationQueries = notificationQueries ?? throw new ArgumentNullException(nameof(notificationQueries));
            _seatQueries = seatQueries ?? throw new ArgumentNullException(nameof(seatQueries));
        }

        public void Execute(Guid sessionId, Guid companyId)
        {
            var aggregates = new List<AggregateRoot>();

            var notifications = _notificationQueries.GetAll(sessionId, companyId);

            if (notifications.Any())
            {
                var seats = _seatQueries.GetAll(sessionId);
                foreach (var seat in seats.Where(a => a.CompanyId == companyId && a.Status == SeatStatus.Valid))
                {
                    var notification = notifications.FirstOrDefault(a => a.SeatId == seat.SeatId);
                    if( notification == null )
                        continue;

                    var aggregate = GetAggregate<BookingNotification>(notification.AggregateId);
                    aggregate.Remove();
                    aggregates.Add(aggregate);

                }
            }

            if(!notifications.Any(a=>a.BookingNotificationType == BookingNotificationType.AgreementToCreate))
                aggregates.Add(BookingNotification.SendAgreementToCreate(sessionId, companyId));

            PublishUncommitedEvents(aggregates.ToArray());
        }
    }
}