using System;
using FluentAssertions;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Seats.Events;
using GestionFormation.CoreDomain.Seats.Exceptions;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.CoreDomain.Sessions.Events;
using GestionFormation.Kernel;
using GestionFormation.Tests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class SeatShould
    {
        [TestMethod]
        public void be_created_when_reserve_seat_in_session()
        {
            var sessiondId = Guid.NewGuid();

            var builder = Aggregate.Make<Session>()
                .AddEvent(new SessionPlanned(sessiondId, 1, Guid.NewGuid(),new DateTime(2017, 01, 01), 5, 5, Guid.NewGuid(), Guid.NewGuid()));

            var session = builder.Create();

            var studentId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            
            var seat = session.BookSeat(studentId, companyId);

            seat.Should().NotBeNull();
            seat.AggregateId.Should().NotBeEmpty();
            seat.UncommitedEvents.GetStream().Should().Contain(new SeatCreated(Guid.Empty, 1, sessiondId, studentId, companyId));
        }
       
        [TestMethod]
        public void raise_seat_canceled_when_cancel_seat()
        {
            var context = CreateTestSeat();
            var seat = context.Builder.Create();
            
            seat.Cancel("TEST");
            seat.UncommitedEvents.GetStream().Should().Contain(new SeatCanceled(Guid.Empty, 1, "TEST"));
        }

        [TestMethod]
        public void raise_seat_refused_when_refuse_seat()
        {
            var context = CreateTestSeat();
            var seat = context.Builder.Create();

            seat.Refuse("TEST");
            seat.UncommitedEvents.GetStream().Should().Contain(new SeatRefused(Guid.Empty, 1, "TEST"));
        }

        [TestMethod]
        public void dont_raise_seat_canceled_if_already_canceled()
        {
            var context = CreateTestSeat();            
            context.Builder.AddEvent((new SeatCanceled(Guid.NewGuid(), 2, "TEST")));

            var seat = context.Builder.Create();
            seat.Cancel("TEST");
            seat.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void dont_raise_seat_refused_if_already_refused()
        {
            var context = CreateTestSeat();
            context.Builder.AddEvent((new SeatRefused(Guid.NewGuid(), 2, "TEST")));

            var seat = context.Builder.Create();
            seat.Refuse("TEST");
            seat.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void not_be_validated_if_not_previously_waiting_for_validation()
        {
            var context = CreateTestSeat();
            context.Builder.AddEvent((new SeatRefused(Guid.NewGuid(), 2, "TEST")));

            var seat = context.Builder.Create();
            Action action = () => seat.Validate();
            action.ShouldThrow<ValidateSeatException>();
        }

        [TestMethod]
        public void raise_AgreementAssociated_when_associate_agreement_on_validate_seat()
        {
            var context = CreateTestSeat();
            var seat = context.Builder.Create();
            seat.Validate();

            var conventionId = Guid.NewGuid();
            seat.AssociateAgreement(conventionId);

            seat.UncommitedEvents.GetStream().Should().Contain(new AgreementAssociated(Guid.Empty, 0, conventionId));
        }

        [TestMethod]
        public void throw_error_when_associate_agreement_that_is_not_validated()
        {
            var context = CreateTestSeat();
            var seat = context.Builder.Create();

            var conventionId = Guid.NewGuid();
            Action action = () => seat.AssociateAgreement(conventionId);

            action.ShouldThrow<AssignAgreementException>();
        }

        [TestMethod]
        public void throw_error_if_assign_agreement_to_unvalided_seat()
        {
            var context = CreateTestSeat();
            var seat = context.Builder.Create();
            seat.Refuse("test");

            var conventionId = Guid.NewGuid();
            Action action = () => seat.AssociateAgreement(conventionId);

            action.ShouldThrow<AssignAgreementException>();
        }

        private class TestSeatContext
        {
            public TestSeatContext(Aggregate.AggregateBuilder<Seat> builder, Guid sessionid, Guid societeId, Guid stagiaireId)
            {
                Builder = builder;
                Sessionid = sessionid;
                SocieteId = societeId;
                StagiaireId = stagiaireId;
            }

            private Guid StagiaireId { get; }
            private Guid SocieteId { get; }
            private Guid Sessionid { get; }
            public Aggregate.AggregateBuilder<Seat> Builder { get; }
        }

        private TestSeatContext CreateTestSeat()
        {
            var stagiaireId = Guid.NewGuid();
            var societeId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            var builder = Aggregate.Make<Seat>().AddEvent(new SeatCreated(Guid.Empty, 1, sessionId, stagiaireId, societeId));

            return new TestSeatContext(builder, sessionId, societeId, stagiaireId);
        }
    }
}