using System;
using FluentAssertions;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Seats.Events;
using GestionFormation.CoreDomain.Seats.Exceptions;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.CoreDomain.Sessions.Events;
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
        public void be_validated_if_previously_waiting_for_validation()
        {
            var context = CreateTestSeat();
            context.Builder.AddEvent((new SeatRefused(Guid.NewGuid(), 2, "TEST")));

            var seat = context.Builder.Create();
            Action action = () => seat.Validate();
            action.ShouldNotThrow<ValidateSeatException>();
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

        [TestMethod]
        public void raise_student_updated_when_updating_student_id()
        {
            var context = CreateTestSeat(true);
            var seat = context.Builder.Create();

            var newStudentId = Guid.NewGuid();
            seat.UpdateStudent(newStudentId);

            seat.UncommitedEvents.GetStream().Should().Contain(new SeatStudentUpdated(Guid.Empty, 0, newStudentId));
        }

        [TestMethod]
        public void dont_raise_student_updated_twice_if_same_student()
        {
            var context = CreateTestSeat();
            var seat = context.Builder.Create();

            seat.UpdateStudent(context.StudentId);
            seat.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void prevent_validation_for_undefined_student()
        {
            var context = CreateTestSeat( true);
            var seat = context.Builder.Create();

            Action action = () => seat.Validate();
            action.ShouldThrow<UndefinedStudentExceptionValidationException>();
        }

        private class TestSeatContext
        {
            public TestSeatContext(Aggregate.AggregateBuilder<Seat> builder, Guid sessionid, Guid companyId, Guid? studentId)
            {
                Builder = builder;
                Sessionid = sessionid;
                CompanyId = companyId;
                StudentId = studentId;
            }

            public Guid? StudentId { get; }
            public Guid CompanyId { get; }
            public Guid Sessionid { get; }
            public Aggregate.AggregateBuilder<Seat> Builder { get; }
        }

        private TestSeatContext CreateTestSeat(bool emptyStudent = false)
        {
            var studentId = emptyStudent ? (Guid?)null : Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            var builder = Aggregate.Make<Seat>().AddEvent(new SeatCreated(Guid.Empty, 1, sessionId, studentId, companyId));

            return new TestSeatContext(builder, sessionId, companyId, studentId);
        }
    }
}