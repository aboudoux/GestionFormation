using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GestionFormation.CoreDomain.BookingNotifications;
using GestionFormation.CoreDomain.BookingNotifications.Events;
using GestionFormation.Tests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class SessionNotificationShould
    {
        [TestMethod]
        public void raise_sessionNotificationCrearted_when_create_session_notification()
        {
            var sessionId = Guid.NewGuid();

            var notif = SessionNotification.Create(sessionId);

            notif.UncommitedEvents.GetStream().Should().HaveCount(1).And
                .Contain(new SessionNotificationCreated(Guid.Empty, 1, sessionId));
        }

        [TestMethod]
        public void raise_seat_to_validate_when_signal_seat_created()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            
            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);

            notif.UncommitedEvents.GetStream().Should().HaveCount(1)                
               .And.Contain(new SeatToValidateNotificationSent(Guid.Empty, 3, context.SessionId, context.Seat(0).CompanyId, context.Seat(0).SeatId));
        }

        [TestMethod]
        public void raise_agreement_to_create_notification_when_signal_seat_valided()
        {
            var context = TestSessionNotification.Create();
            var notif = context.Builder.Create();

            context.AddSeat();
            notif.SignalSeatCreated(context.Seat(0).SeatId, context.Seat(0).CompanyId);
            notif.SignalSeatValidated(context.Seat(0).SeatId, context.Seat(0).CompanyId);

            var removedEvent = notif.UncommitedEvents.GetStream().OfType<SeatToValidateNotificationSent>().First();

            notif.UncommitedEvents.GetStream().Should().HaveCount(3)
                .And.Contain(new SeatToValidateNotificationSent(Guid.Empty, 1, context.SessionId, context.Seat(0).CompanyId, context.Seat(0).SeatId))
                .And.Contain(new AgreementToCreateNotificationSent(Guid.Empty, 1, context.SessionId, context.Seat(0).CompanyId))
                .And.Contain(new BookingNotificationRemoved(Guid.Empty, 1, removedEvent.NotificationId));
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

            var removedEvent1 = notif.UncommitedEvents.GetStream().OfType<SeatToValidateNotificationSent>().First(a=>a.SeatId == context.Seat(0).SeatId);
            var removedEvent2 = notif.UncommitedEvents.GetStream().OfType<SeatToValidateNotificationSent>().First(a=>a.SeatId == context.Seat(1).SeatId);

            notif.UncommitedEvents.GetStream().Should().HaveCount(5)
                .And.Contain(new SeatToValidateNotificationSent(Guid.Empty, 1, context.SessionId, context.Seat(0).CompanyId, context.Seat(0).SeatId))
                .And.Contain(new SeatToValidateNotificationSent(Guid.Empty, 1, context.SessionId, context.Seat(1).CompanyId, context.Seat(1).SeatId))
                .And.Contain(new AgreementToCreateNotificationSent(Guid.Empty, 1, context.SessionId, context.Seat(0).CompanyId))
                .And.Contain(new BookingNotificationRemoved(Guid.Empty, 1, removedEvent1.NotificationId))
                .And.Contain(new BookingNotificationRemoved(Guid.Empty, 1, removedEvent2.NotificationId));
        }

        private class TestSessionNotification
        {
            private readonly List<CreatedSeat> _createdSeats = new List<CreatedSeat>();

            public Guid SessionId { get; }

            public Aggregate.AggregateBuilder<SessionNotification> Builder { get; }

            public TestSessionNotification()
            {
                SessionId = Guid.NewGuid();
                Builder = Aggregate.Make<SessionNotification>().AddEvent(new SessionNotificationCreated(Guid.NewGuid(), 1, SessionId));
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
}