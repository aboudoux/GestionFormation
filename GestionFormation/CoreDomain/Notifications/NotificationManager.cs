using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Notifications.Events;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Notifications
{
    public class NotificationManager : AggregateRoot
    {
        private Guid _sessionId;
        private readonly List<NotificationEvent> _notifications = new List<NotificationEvent>();
        private readonly List<SeatState> _seatStates = new List<SeatState>();

        public NotificationManager(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            player.Add<NotificationManagerCreated>(a => _sessionId = a.SessionId)
                .Add<SeatToValidateNotificationSent>(a => _notifications.Add(a))
                .Add<AgreementToCreateNotificationSent>(a => _notifications.Add(a))                
                .Add<AgreementToSignNotificationSent>(a=>_notifications.Add(a))
                .Add<NotificationRemoved>(a => _notifications.RemoveAll(b => b.NotificationId == a.NotificationId))
                .Add<CreateSeatSignaled>(a=> _seatStates.Add(new SeatState(a.SeatId, a.CompanyId)))
                .Add<ValidateSeatSignaled>(a=> UpdateSeatState(a.SeatId, SeatStatus.Valid))
                .Add<RefuseSeatSignaled>(a => UpdateSeatState(a.SeatId, SeatStatus.Refused))
                .Add<CancelSeatSignaled>(a => UpdateSeatState(a.SeatId, SeatStatus.Canceled))
                .Add<AgreementAssociatedToSeatSignaled>(a=> UpdateSeatAgreement(a.SeatId, a.AgreementId))
                ;
        }

        private void UpdateSeatState(Guid seatId, SeatStatus status)
        {
            var seat = _seatStates.FirstOrDefault(b => b.SeatId == seatId);
            if (seat != null)
                seat.Status = status;
        }

        private void UpdateSeatAgreement(Guid seatId, Guid? agreementId)
        {
            var seat = _seatStates.FirstOrDefault(b => b.SeatId == seatId);
            if (seat != null)
                seat.AssociatedAgreement = agreementId;
        }

        public static NotificationManager Create(Guid sessionId)
        {
            sessionId.EnsureNotEmpty(nameof(sessionId));
            var notification = new NotificationManager(History.Empty);
            notification.AggregateId = Guid.NewGuid();
            notification.UncommitedEvents.Add(new NotificationManagerCreated(notification.AggregateId, 1, sessionId));
            return notification;
        }

        public void SignalSeatCreated(Guid seatId, Guid companyId, bool sendNotification = true)
        {
            GuidAssert.AreNotEmpty(seatId, companyId);

            RaiseEvent(new CreateSeatSignaled(AggregateId, GetNextSequence(), seatId, companyId));

            if (sendNotification)
                RaiseEvent(new SeatToValidateNotificationSent(AggregateId, GetNextSequence(), _sessionId, companyId, seatId, Guid.NewGuid()));
        }

        public void SignalSeatValidated(Guid seatId, Guid companyId)
        {
            GuidAssert.AreNotEmpty(seatId, companyId);

            RaiseEvent(new ValidateSeatSignaled(AggregateId, GetNextSequence(), seatId));

            if(_notifications.OfType<AgreementToCreateNotificationSent>().All(a => a.CompanyId != companyId))
                RaiseEvent(new AgreementToCreateNotificationSent(AggregateId, GetNextSequence(), _sessionId, companyId, Guid.NewGuid()));

            RemoveNotifications<SeatToValidateNotificationSent>(a => a.SeatId == seatId);         
        }
      
        public void SignalAgreementAssociated(Guid agreementId, Guid seatId, Guid companyId)
        {
            GuidAssert.AreNotEmpty(agreementId, seatId, companyId);

            RaiseEvent(new AgreementAssociatedToSeatSignaled(AggregateId, GetNextSequence(), seatId, agreementId));

            if (_notifications.OfType<AgreementToSignNotificationSent>().All(a => a.CompanyId != companyId || a.AgreementId != agreementId))
                RaiseEvent(new AgreementToSignNotificationSent(AggregateId, GetNextSequence(), _sessionId, companyId, agreementId, Guid.NewGuid()));

            RemoveNotifications<AgreementToCreateNotificationSent>(a => a.CompanyId == companyId);
        }       

        public void SignalAgreementSigned(Guid agreementId)
        {
            GuidAssert.AreNotEmpty(agreementId);
            RemoveNotifications<AgreementToSignNotificationSent>(a => a.AgreementId == agreementId);            
        }

        public void SignalSeatRefused(Guid seatId, Guid companyId)
        {
            CancelOrRefuse(seatId, companyId, new RefuseSeatSignaled(AggregateId, GetNextSequence(), seatId));
        }

        public void SignalSeatCanceled(Guid seatId, Guid companyId)
        {
            CancelOrRefuse(seatId, companyId, new CancelSeatSignaled(AggregateId, GetNextSequence(), seatId));           
        }

        public void Remove(Guid notificationId)
        {
            GuidAssert.AreNotEmpty(notificationId);
            RaiseEvent(new NotificationRemoved(AggregateId, GetNextSequence(), notificationId));
        }

        private void CancelOrRefuse(Guid seatId, Guid companyId, DomainEvent eventToRaise)
        {
            if (eventToRaise == null) throw new ArgumentNullException(nameof(eventToRaise));
            GuidAssert.AreNotEmpty(seatId, companyId);
            RaiseEvent(eventToRaise);

            if (!_seatStates.Any(a => a.CompanyId == companyId && a.Status == SeatStatus.Valid && !a.AssociatedAgreement.HasValue))
                RemoveNotifications<AgreementToCreateNotificationSent>(a => a.CompanyId == companyId);

            RemoveNotifications<SeatToValidateNotificationSent>(a => a.SeatId == seatId);
        }

        public void SignalAgreementRevoked(Guid agreementId)
        {
            GuidAssert.AreNotEmpty(agreementId);

            RemoveNotifications<AgreementToSignNotificationSent>(a => a.AgreementId == agreementId);

            var companyId = _seatStates.FirstOrDefault(a => a.AssociatedAgreement == agreementId)?.CompanyId;
            if(!companyId.HasValue) return;      
            
            if (_seatStates.Any(a => a.CompanyId == companyId && a.Status == SeatStatus.Valid))
                RaiseEvent(new AgreementToCreateNotificationSent(AggregateId, GetNextSequence(), _sessionId, companyId.Value, Guid.NewGuid()));
        }

        private void RemoveNotifications<T>(Func<T, bool> predicate)
        where T : NotificationEvent
        {
            foreach (var notification in _notifications.OfType<T>().Where(predicate).ToList())
                RaiseEvent(new NotificationRemoved(AggregateId, GetNextSequence(), notification.NotificationId));
        }

        private class SeatState
        {
            public SeatState(Guid seatId, Guid companyId)
            {
                SeatId = seatId;
                CompanyId = companyId;
                Status = SeatStatus.ToValidate;
            }
            public Guid SeatId { get; }
            public Guid CompanyId { get; }
            public SeatStatus Status { get; set; }
            public Guid? AssociatedAgreement { get; set; }
        }
    }   

    public class CreateSeatSignaled : SignaledEvent
    {
        public Guid CompanyId { get; }

        public CreateSeatSignaled(Guid aggregateId, int sequence, Guid seatId, Guid companyId) : base(aggregateId, sequence, seatId)
        {
            CompanyId = companyId;
        }

        protected override string Description => "Création de place signalée au système de notification";
    }

    public class ValidateSeatSignaled : SignaledEvent
    {
        public ValidateSeatSignaled(Guid aggregateId, int sequence, Guid seatId) : base(aggregateId, sequence, seatId)
        {
        }

        protected override string Description => "Validation de place signalé au système de notification";
    }

    public class RefuseSeatSignaled : SignaledEvent
    {
        public RefuseSeatSignaled(Guid aggregateId, int sequence, Guid seatId) : base(aggregateId, sequence, seatId)
        {
        }
        protected override string Description => "Refus de place signalée au système de notification";
    }

    public class CancelSeatSignaled : SignaledEvent
    {
        public CancelSeatSignaled(Guid aggregateId, int sequence, Guid seatId) : base(aggregateId, sequence, seatId)
        {
        }

        protected override string Description => "Annulation de place signalée au système de notification";
    }

    public class AgreementAssociatedToSeatSignaled : SignaledEvent
    {
        public Guid AgreementId { get; }

        public AgreementAssociatedToSeatSignaled(Guid aggregateId, int sequence, Guid seatId, Guid agreementId) : base(aggregateId, sequence, seatId)
        {
            AgreementId = agreementId;
        }

        protected override string Description => "Convention associée à une place signalée au système de notification";
    }

    public abstract class SignaledEvent : DomainEvent
    {
        public Guid SeatId { get; }

        protected SignaledEvent(Guid aggregateId, int sequence, Guid seatId) : base(aggregateId, sequence)
        {
            SeatId = seatId;
        }
    }
}