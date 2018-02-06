using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.BookingNotifications.Events;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.BookingNotifications
{
    public class SessionNotification : AggregateRoot
    {
        private Guid _sessionId;
        private readonly List<BookingNotificationEvent> _notifications = new List<BookingNotificationEvent>();

        public SessionNotification(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            player.Add<SessionNotificationCreated>(a => _sessionId = a.SessionId)
                .Add<SeatToValidateNotificationSent>(a => _notifications.Add(a))
                .Add<AgreementToCreateNotificationSent>(a => _notifications.Add(a))
                .Add<BookingNotificationRemoved>(a =>_notifications.RemoveAll(b => b.NotificationId == a.NotificationId));
        }

        public static SessionNotification Create(Guid sessionId)
        {
            sessionId.EnsureNotEmpty(nameof(sessionId));
            var notification = new SessionNotification(History.Empty);
            notification.AggregateId = Guid.NewGuid();
            notification.UncommitedEvents.Add(new SessionNotificationCreated(notification.AggregateId, 1, sessionId));
            return notification;
        }

        public void SignalSeatCreated(Guid seatId, Guid companyId, bool sendNotification = true)
        {
            GuidAssert.AreNotEmpty(seatId, companyId);

            if (sendNotification)
                RaiseEvent(new SeatToValidateNotificationSent(AggregateId, GetNextSequence(), _sessionId, companyId, seatId));
        }

        public void SignalSeatValidated(Guid seatId, Guid companyId)
        {
            GuidAssert.AreNotEmpty(seatId, companyId);

            if(_notifications.OfType<AgreementToCreateNotificationSent>().All(a => a.CompanyId != companyId))
                RaiseEvent(new AgreementToCreateNotificationSent(AggregateId, GetNextSequence(), _sessionId, companyId));

            foreach (var notification in _notifications.OfType<SeatToValidateNotificationSent>().Where(a=>a.CompanyId == companyId).ToList())
                RaiseEvent(new BookingNotificationRemoved(AggregateId, GetNextSequence(), notification.NotificationId));            
        }

        public void SignalSeatRefused(Guid seatId, Guid companyId)
        {
            GuidAssert.AreNotEmpty(seatId, companyId);
        }

        public void SignalSeatCanceled(Guid seatId, Guid companyId)
        {
            GuidAssert.AreNotEmpty(seatId, companyId);
        }

        public void SignalAgreementAssociated(Guid agreementId, Guid seatId, Guid companyId)
        {
            GuidAssert.AreNotEmpty(agreementId, seatId, companyId);
        }

        public void SignalAgreementRevoked(Guid agreementId)
        {
            GuidAssert.AreNotEmpty(agreementId);
        }

        public void SignalAgreementSigned(Guid agreementId)
        {
            GuidAssert.AreNotEmpty(agreementId);
        }
    }

    public class SessionNotificationCreated : DomainEvent
    {
        public Guid SessionId { get; }

        public SessionNotificationCreated(Guid aggregateId, int sequence, Guid sessionId) : base(aggregateId, sequence)
        {
            SessionId = sessionId;
        }

        protected override string Description => "Notification de session créée";
    }

    /*
    public class SeatCreatedSignaled : DomainEvent
    {      
        public SeatCreatedSignaled(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {          
        }

        protected override string Description => "Placé créée signalée";
    }

    public class SeatValidatedSignaled : DomainEvent
    {
        public SeatValidatedSignaled(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Place validée signalée";
    }
    */
}