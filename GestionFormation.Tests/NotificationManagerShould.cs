using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GestionFormation.CoreDomain.Notifications;
using GestionFormation.CoreDomain.Notifications.Events;
using GestionFormation.Kernel;
using GestionFormation.Tests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class NotificationManagerShould
    {
        [TestMethod]
        public void raise_sessionNotificationCreated_when_create_session_notification()
        {
            var sessionId = Guid.NewGuid();

            var notif = NotificationManager.Create(sessionId);

            notif.UncommitedEvents.GetStream().Should().HaveCount(1).And
                .Contain(new NotificationManagerCreated(Guid.Empty, 1, sessionId));
        }

        [TestMethod]
        public void raise_seat_to_validate_when_signal_seat_created()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            
            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);

          var handler = new FakeNotificationHanlder(notif.UncommitedEvents);
            handler.Notifications.Should().HaveCount(1)
                .And.Contain(a => a.Type == NotificationType.SeatToValidate);

        }

        [TestMethod]
        public void dont_remove_ToValidateNotification()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            context.AddSeat(context.Seat(0).CompanyId);

            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatCreated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            notif.SignalSeatValidated(context.Seat(1).SeatId, context.Seat(1).CompanyId);            

            var handler = new FakeNotificationHanlder(notif.UncommitedEvents);
            handler.Notifications.Should().HaveCount(2)
                .And.Contain(a => a.Type == NotificationType.SeatToValidate)
                .And.Contain(a => a.Type == NotificationType.AgreementToCreate);
        }

        [TestMethod]
        public void raise_agreement_to_create_notification_when_signal_seat_valided()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatValidated(context.Seat(0).SeatId, context.Seat(0).CompanyId);

            var handler = new FakeNotificationHanlder(notif.UncommitedEvents);
            handler.Notifications.Should().HaveCount(1).And.Contain(a => a.Type == NotificationType.AgreementToCreate);
        }

        [TestMethod]
        public void not_raise_AgreementToCreateNotificationSent_twice_for_same_company()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            context.AddSeat(context.Seat(0).CompanyId);

            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatCreated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            notif.SignalSeatValidated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatValidated(context.Seat(1).SeatId, context.Seat(1).CompanyId);
           
            var handler = new FakeNotificationHanlder(notif.UncommitedEvents);
            handler.Notifications.Should().HaveCount(1).And.Contain(a => a.Type == NotificationType.AgreementToCreate);
        }

        [TestMethod]
        public void raise_AgreementToSignNotificationSent_when_creating_agreement_for_one_seat()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            var agreementId = Guid.NewGuid();

            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatValidated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalAgreementAssociated(agreementId, context.Seat(0).SeatId, context.Seat(0).CompanyId);

            var handler = new FakeNotificationHanlder(notif.UncommitedEvents);
            handler.Notifications.Should().HaveCount(1).And.Contain(a => a.Type == NotificationType.AgreementToSign);
        }

        [TestMethod]
        public void dont_raise_AgreementToSignNotificationSent_twice_when_creating_agreement_for_two_seat()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            context.AddSeat(context.Seat(0).CompanyId);

            var agreementId = Guid.NewGuid();

            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatCreated(context.Seat(1).SeatId, context.Seat(1).CompanyId);
            notif.SignalSeatValidated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatValidated(context.Seat(1).SeatId, context.Seat(1).CompanyId);
            notif.SignalAgreementAssociated(agreementId, context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalAgreementAssociated(agreementId, context.Seat(1).SeatId, context.Seat(1).CompanyId);

            var handler = new FakeNotificationHanlder(notif.UncommitedEvents);
            handler.Notifications.Should().HaveCount(1).And.Contain(a => a.Type == NotificationType.AgreementToSign);
        }

        [TestMethod]
        public void raise_remove_AgreementToSignNotificationSent_when_signal_agreement_signed()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            var agreementId = Guid.NewGuid();

            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatValidated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalAgreementAssociated(agreementId, context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalAgreementSigned(agreementId);
       
            var handler = new FakeNotificationHanlder(notif.UncommitedEvents);
            handler.Notifications.Should().BeEmpty();
        }        

        [TestMethod]
        public void remove_SeatToValidate_when_refuse_seat()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatRefused(context.Seat(0).SeatId, context.Seat(0).CompanyId);          

            var handler = new FakeNotificationHanlder(notif.UncommitedEvents);
            handler.Notifications.Should().BeEmpty();
        }

        [TestMethod]
        public void remove_SeatToValidate_when_cancel_seat()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatCanceled(context.Seat(0).SeatId, context.Seat(0).CompanyId);

            var handler = new FakeNotificationHanlder(notif.UncommitedEvents);
            handler.Notifications.Should().BeEmpty();
        }

        [TestMethod]
        public void remove_agreementToCreated_if_all_place_refused_or_canceled()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            context.AddSeat(context.Seat(0).CompanyId);

            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatCreated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            notif.SignalSeatValidated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatValidated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            notif.SignalSeatCanceled(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatRefused(context.Seat(1).SeatId, context.Seat(1).CompanyId);            

            var handler = new FakeNotificationHanlder(notif.UncommitedEvents);
            handler.Notifications.Should().BeEmpty();
        }

        [TestMethod]
        public void dont_remove_agreementToCreated_if_one_place_already_valided()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            context.AddSeat(context.Seat(0).CompanyId);

            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatCreated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            notif.SignalSeatValidated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatValidated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            notif.SignalSeatCanceled(context.Seat(0).SeatId, context.Seat(0).CompanyId);
          
            var handler = new FakeNotificationHanlder(notif.UncommitedEvents);
            handler.Notifications.Should().HaveCount(1)
                .And.Contain(a => a.Type == NotificationType.AgreementToCreate)                
                ;
        }

        [TestMethod]
        public void re_raise_agreementToCreated_if_created_agreement_is_revoked()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            context.AddSeat(context.Seat(0).CompanyId);

            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatCreated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            notif.SignalSeatValidated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatValidated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            var agreementId = Guid.NewGuid();            
            notif.SignalAgreementAssociated(agreementId, context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalAgreementAssociated(agreementId, context.Seat(1).SeatId, context.Seat(1).CompanyId);            

            notif.SignalSeatCanceled(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalAgreementRevoked(agreementId);
           
            var handler = new FakeNotificationHanlder(notif.UncommitedEvents);
            handler.Notifications.Should().HaveCount(1)
                .And.Contain(a => a.Type == NotificationType.AgreementToCreate);
        }

        [TestMethod]
        public void not_remove_agreement_to_sign_when_removing_valided_seat_where_agreement_not_created()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            context.AddSeat(context.Seat(0).CompanyId);

            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId, false);
            notif.SignalSeatCreated(context.Seat(1).SeatId, context.Seat(1).CompanyId, false);

            notif.SignalSeatValidated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatValidated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            var agreementId = Guid.NewGuid();
            notif.SignalAgreementAssociated(agreementId, context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalAgreementAssociated(agreementId, context.Seat(1).SeatId, context.Seat(1).CompanyId);

            context.AddSeat(context.Seat(0).CompanyId);
            notif.SignalSeatCreated(context.Seat(2).SeatId, context.Seat(2).CompanyId, false);
            notif.SignalSeatValidated(context.Seat(2).SeatId, context.Seat(2).CompanyId);
            notif.SignalSeatRefused(context.Seat(2).SeatId, context.Seat(2).CompanyId);
           
            var hanlder = new FakeNotificationHanlder(notif.UncommitedEvents);
            hanlder.Notifications.Should().HaveCount(1).And.Contain(a => a.Type == NotificationType.AgreementToSign);
        }

        [TestMethod]
        public void remove_notification_when_agreement_created_on_revoked_agreement()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            context.AddSeat(context.Seat(0).CompanyId);

            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatCreated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            notif.SignalSeatValidated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatValidated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            var agreementId1 = Guid.NewGuid();
            notif.SignalAgreementAssociated(agreementId1, context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalAgreementAssociated(agreementId1, context.Seat(1).SeatId, context.Seat(1).CompanyId);

            notif.SignalSeatCanceled(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalAgreementRevoked(agreementId1);

            var agreementId2 = Guid.NewGuid();
            notif.SignalAgreementAssociated(agreementId2, context.Seat(1).SeatId, context.Seat(1).CompanyId);

            var hanlder = new FakeNotificationHanlder(notif.UncommitedEvents);
            hanlder.Notifications.Should().HaveCount(1).And.Contain(a => a.Type == NotificationType.AgreementToSign && a.AgreementId == agreementId2);
        }

        [TestMethod]
        public void create_2_agreementToSignNotification_for_different_agreement_at_same_company()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            context.AddSeat(context.Seat(0).CompanyId);

            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatCreated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            notif.SignalSeatValidated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatValidated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            var agreementId1 = Guid.NewGuid();
            var agreementId2 = Guid.NewGuid();

            notif.SignalAgreementAssociated(agreementId1, context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalAgreementAssociated(agreementId2, context.Seat(1).SeatId, context.Seat(1).CompanyId);

            var hanlder = new FakeNotificationHanlder(notif.UncommitedEvents);
            hanlder.Notifications.Should().HaveCount(2)
                .And.Contain(a => a.Type == NotificationType.AgreementToSign && a.AgreementId == agreementId1)
                .And.Contain(a => a.Type == NotificationType.AgreementToSign && a.AgreementId == agreementId2);
        }

        [TestMethod]
        public void raise_remove_AgreementToSignNotificationSent_when_signal_agreement_signed_for_secon_agreement()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            context.AddSeat(context.Seat(0).CompanyId);

            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatCreated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            notif.SignalSeatValidated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatValidated(context.Seat(1).SeatId, context.Seat(1).CompanyId);

            var agreementId1 = Guid.NewGuid();
            var agreementId2 = Guid.NewGuid();

            notif.SignalAgreementAssociated(agreementId1, context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalAgreementAssociated(agreementId2, context.Seat(1).SeatId, context.Seat(1).CompanyId);

            notif.SignalAgreementSigned(agreementId1);
            var hanlder = new FakeNotificationHanlder(notif.UncommitedEvents);
            hanlder.Notifications.Should().HaveCount(1)
                .And.Contain(a => a.Type == NotificationType.AgreementToSign && a.AgreementId == agreementId2);
        }

        private class TestSessionNotification
        {
            private readonly List<CreatedSeat> _createdSeats = new List<CreatedSeat>();

            public Guid SessionId { get; }

            public Aggregate.AggregateBuilder<NotificationManager> Builder { get; }

            private TestSessionNotification()
            {
                SessionId = Guid.NewGuid();
                Builder = Aggregate.Make<NotificationManager>().AddEvent(new NotificationManagerCreated(Guid.NewGuid(), 1, SessionId));
            }

            public void AddSeat(Guid? companyId = null)
            {
                _createdSeats.Add(new CreatedSeat(companyId));                
            }

            public CreatedSeat Seat(int indice)
            {
                return _createdSeats[indice];
            }

            public static  TestSessionNotification Create()
            {
                return new TestSessionNotification();
            }
        }

        private class CreatedSeat
        {
            public Guid SeatId { get; }
            public Guid CompanyId { get; }

            public CreatedSeat(Guid? companyId)
            {
                SeatId = Guid.NewGuid();
                CompanyId = companyId.HasValue ? companyId.Value : Guid.NewGuid();
            }
        }
    }


    public class FakeNotificationHanlder
    {
        public List<FakeNotification> Notifications { get; }

        public FakeNotificationHanlder(EventStream events)
        {
            Notifications = new List<FakeNotification>();

            foreach (var @event in events.GetStream())
            {
                if(@event is NotificationEvent || @event is NotificationRemoved)
                    Handle((dynamic)@event);
            }
        }

        private void Handle(SeatToValidateNotificationSent @event)
        {
            Notifications.Add(new FakeNotification(@event.NotificationId, NotificationType.SeatToValidate));
        }

        private void Handle(AgreementToCreateNotificationSent @event)
        {
            Notifications.Add(new FakeNotification(@event.NotificationId, NotificationType.AgreementToCreate));
        }

        private void Handle(AgreementToSignNotificationSent @event)
        {
            Notifications.Add(new FakeNotification(@event.NotificationId, NotificationType.AgreementToSign, @event.AgreementId));
        }

        private void Handle(NotificationRemoved @event)
        {
            Notifications.RemoveAll(a => a.NotificationId == @event.NotificationId);
        }
    }

    public class FakeNotification
    {
        public Guid NotificationId { get; }
        public NotificationType Type { get; }
        public Guid? AgreementId { get; }

        public FakeNotification(Guid notificationId, NotificationType type, Guid? agreementId = null)
        {
            NotificationId = notificationId;
            Type = type;
            AgreementId = agreementId;
        }
    }

}