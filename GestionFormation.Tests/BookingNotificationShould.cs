using System;
using FluentAssertions;
using GestionFormation.CoreDomain.BookingNotifications;
using GestionFormation.CoreDomain.BookingNotifications.Events;
using GestionFormation.CoreDomain.BookingNotifications.Exceptions;
using GestionFormation.Tests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class BookingNotificationShould
    {
        [TestMethod]
        public void create_notification_for_seat_to_validate()
        {
            var sessionId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var seatId = Guid.NewGuid();
            var notification = BookingNotification.SendSeatToValidate(sessionId, companyId, seatId);
            notification.UncommitedEvents.GetStream().Should().Contain(new SeatToValidateNotificationSent(Guid.Empty, 0, sessionId, companyId, seatId));
        }

        [TestMethod]
        public void change_agreementToCreate_to_agreementToSign()
        {
            var context = TestNotification.Create();
            context.Builder.AddEvent(new AgreementToCreateNotificationSent(Guid.Empty, 2, context.SessionId, context.CompanyId));
            var notif = context.Builder.Create();

            var conventionId = Guid.NewGuid();
            notif.ChangeToAgreementToSign(conventionId);
            notif.UncommitedEvents.GetStream().Should().Contain(new AgreementToSignNotificationSent(Guid.Empty, 0, context.SessionId, context.CompanyId, conventionId));
        }

        [TestMethod]
        public void not_pass_from_seatToValidate_to_agreementToSign()
        {
            var context = TestNotification.Create();
            var notif = context.Builder.Create();

            Action action = () => notif.ChangeToAgreementToSign(Guid.NewGuid());
            action.ShouldThrow<ChangeNotificationException>();
        }

        [TestMethod]
        public void not_pass_from_agreementToSign_to_agreementToCreate()
        {
            var context = TestNotification.Create();
            context.Builder.AddEvent(new AgreementToSignNotificationSent(Guid.Empty, 1, context.SessionId, context.CompanyId, Guid.NewGuid()));

            var notif = context.Builder.Create();
            Action action = () => notif.ChangeToAgreementToSign(Guid.NewGuid());
            action.ShouldThrow<ChangeNotificationException>();
        }

        private class TestNotification
        {
            public Guid SeatId { get; }
            public Guid CompanyId { get; }
            public Guid SessionId { get; }

            public Aggregate.AggregateBuilder<BookingNotification> Builder { get; }

            private TestNotification()
            {
                SeatId = Guid.NewGuid();
                CompanyId = Guid.NewGuid();
                SessionId = Guid.NewGuid();

                Builder = Aggregate.Make<BookingNotification>().AddEvent(new SeatToValidateNotificationSent(Guid.NewGuid(), 1, SessionId, CompanyId, SeatId));
            }

            public static TestNotification Create()
            {
                return new TestNotification();
            }
        }
    }
}