using System;
using GestionFormation.CoreDomain.BookingNotifications.Events;
using GestionFormation.CoreDomain.BookingNotifications.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.BookingNotifications
{
    public class BookingNotification : AggregateRoot
    {
        private Guid _sessiond;
        private Guid _companyId;

        private BookingNotificationType _currentType;

        public BookingNotification(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            player.Add<SeatToValidateSent>(a =>
                {
                    _companyId = a.CompanyId;
                    _sessiond = a.SessionId;
                    _currentType = BookingNotificationType.PlaceToValidate;
                })
                .Add<AgreementToCreateSent>(a =>
                {
                    _currentType = BookingNotificationType.AgreementToCreate;
                    _sessiond = a.SessionId;
                    _companyId = a.CompanyId;
                })
                .Add<AgreementToSignSent>(a => _currentType = BookingNotificationType.AgreementToSign);
        }

        public static BookingNotification SendSeatToValidate(Guid sessionId, Guid companyId, Guid seatId)
        {   
            sessionId.EnsureNotEmpty(nameof(sessionId));
            companyId.EnsureNotEmpty(nameof(companyId));
            seatId.EnsureNotEmpty(nameof(seatId));

            var notification = new BookingNotification(History.Empty);
            notification.AggregateId = Guid.NewGuid();
            notification.UncommitedEvents.Add(new SeatToValidateSent(notification.AggregateId, 1, sessionId, companyId, seatId));
            return notification;
        }

        public static BookingNotification SendAgreementToCreate(Guid sessionId, Guid companyId)
        {
            sessionId.EnsureNotEmpty(nameof(sessionId));
            companyId.EnsureNotEmpty(nameof(companyId));

            var notification = new BookingNotification(History.Empty);
            notification.AggregateId = Guid.NewGuid();
            notification.UncommitedEvents.Add(new AgreementToCreateSent(notification.AggregateId, 1, sessionId, companyId));
            return notification;
        }

        public void ChangeToAgreementToCreate()
        {
            RaiseEvent(new AgreementToCreateSent(AggregateId, GetNextSequence(), _sessiond, _companyId));
        }

        public void ChangeToAgreementToSign(Guid aggrementId)
        {
            aggrementId.EnsureNotEmpty(nameof(aggrementId));

            if(_currentType != BookingNotificationType.AgreementToCreate)
                throw new ChangeNotificationException();

            RaiseEvent(new AgreementToSignSent(AggregateId, GetNextSequence(), _sessiond, _companyId, aggrementId));
        }

        public void Remove()
        {
            RaiseEvent(new BookingNotificationRemoved(AggregateId, GetNextSequence()));
        }
    }
}
