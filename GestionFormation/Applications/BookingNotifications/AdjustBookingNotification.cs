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
    public class AdjustBookingNotification : ActionCommand
    {
        private readonly INotificationQueries _notificationQueries;
        private readonly ISeatQueries _seatQueries;

        public AdjustBookingNotification(EventBus eventBus, INotificationQueries notificationQueries, ISeatQueries seatQueries) : base(eventBus)
        {
            _notificationQueries = notificationQueries ?? throw new ArgumentNullException(nameof(notificationQueries));
            _seatQueries = seatQueries ?? throw new ArgumentNullException(nameof(seatQueries));
        }

        public void Execute(Guid seatId)
        {
            var aggregatesToValidate = new List<AggregateRoot>();

            var seat = _seatQueries.GetSeat(seatId);
            if(seat == null)
                throw new Exception("Impossible de trouver la place avec l'identifiant " + seatId);
            
            var notifications = _notificationQueries.GetAll(seat.SessionId, seat.CompanyId);
            var allSeatForSession = _seatQueries.GetAll(seat.SessionId).ToList();

            foreach (var notification in notifications)
            {
                if (notification.BookingNotificationType == BookingNotificationType.AgreementToSign && allSeatForSession.Any(a => a.CompanyId == seat.CompanyId && a.Status == SeatStatus.Valid && a.AgreementId.HasValue))
                    continue;

                var notif = GetAggregate<BookingNotification>(notification.AggregateId);
                notif.Remove();
                aggregatesToValidate.Add(notif);
            }
            PublishUncommitedEvents(aggregatesToValidate.ToArray());

            var sendSeatToValidateCommand = new SendSeatToValidateNotification(EventBus);
            var sendAgreementToCreateCommand = new SendAgreementToCreateNotification(EventBus, _notificationQueries, _seatQueries);

            foreach (var seatResult in allSeatForSession.Where(a=> a.CompanyId == seat.CompanyId && a.Status == SeatStatus.ToValidate))
                sendSeatToValidateCommand.Execute(seatResult.SessionId, seatResult.CompanyId, seatResult.SeatId);

            if(allSeatForSession.Any(a => a.CompanyId == seat.CompanyId && a.Status == SeatStatus.Valid && !a.AgreementId.HasValue))
                sendAgreementToCreateCommand.Execute(seat.SessionId, seat.CompanyId);          
        }
    }
}