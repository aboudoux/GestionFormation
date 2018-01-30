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
    public class PlacesShould
    {
        [TestMethod]
        public void be_created_when_reserve_place_in_session()
        {
            var sessiondId = Guid.NewGuid();

            var builder = Aggregate.Make<Session>()
                .AddEvent(new SessionPlanned(sessiondId, 1, Guid.NewGuid(),new DateTime(2017, 01, 01), 5, 5, Guid.NewGuid(), Guid.NewGuid()));

            var session = builder.Create();

            var stagiaireId = Guid.NewGuid();
            var societeId = Guid.NewGuid();
            
            var place = session.BookSeat(stagiaireId, societeId);

            place.Should().NotBeNull();
            place.AggregateId.Should().NotBeEmpty();
            place.UncommitedEvents.GetStream().Should().Contain(new SeatCreated(Guid.Empty, 1, sessiondId, stagiaireId, societeId));
        }
       
        [TestMethod]
        public void raise_place_canceled_when_cancel_place()
        {
            var context = CreateTestPlace();
            var place = context.Builder.Create();
            
            place.Cancel("TEST");
            place.UncommitedEvents.GetStream().Should().Contain(new SeatCanceled(Guid.Empty, 1, "TEST"));
        }

        [TestMethod]
        public void raise_place_refused_when_refuse_place()
        {
            var context = CreateTestPlace();
            var place = context.Builder.Create();

            place.Refuse("TEST");
            place.UncommitedEvents.GetStream().Should().Contain(new SeatRefused(Guid.Empty, 1, "TEST"));
        }

        [TestMethod]
        public void dont_raise_place_canceled_if_already_canceled()
        {
            var context = CreateTestPlace();            
            context.Builder.AddEvent((new SeatCanceled(Guid.NewGuid(), 2, "TEST")));

            var place = context.Builder.Create();
            place.Cancel("TEST");
            place.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void dont_raise_place_refused_if_already_refused()
        {
            var context = CreateTestPlace();
            context.Builder.AddEvent((new SeatRefused(Guid.NewGuid(), 2, "TEST")));

            var place = context.Builder.Create();
            place.Refuse("TEST");
            place.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void not_be_validated_if_not_previously_waiting_for_validation()
        {
            var context = CreateTestPlace();
            context.Builder.AddEvent((new SeatRefused(Guid.NewGuid(), 2, "TEST")));

            var place = context.Builder.Create();
            Action action = () => place.Validate();
            action.ShouldThrow<ValidateSeatException>();
        }

        [TestMethod]
        public void raise_conventionAssociated_when_associate_convention_on_validate_place()
        {
            var context = CreateTestPlace();
            var place = context.Builder.Create();
            place.Validate();

            var conventionId = Guid.NewGuid();
            place.AssociateAgreement(conventionId);

            place.UncommitedEvents.GetStream().Should().Contain(new AgreementAssociated(Guid.Empty, 0, conventionId));
        }

        [TestMethod]
        public void throw_error_when_associate_convention_that_is_not_validated()
        {
            var context = CreateTestPlace();
            var place = context.Builder.Create();

            var conventionId = Guid.NewGuid();
            Action action = () => place.AssociateAgreement(conventionId);

            action.ShouldThrow<AssignAgreementException>();
        }

        [TestMethod]
        public void throw_error_if_assign_convention_to_unvalided_place()
        {
            var context = CreateTestPlace();
            var place = context.Builder.Create();
            place.Refuse("test");

            var conventionId = Guid.NewGuid();
            Action action = () => place.AssociateAgreement(conventionId);

            action.ShouldThrow<AssignAgreementException>();
        }

        private class TestPlaceContext
        {
            public TestPlaceContext(Aggregate.AggregateBuilder<Seat> builder, Guid sessionid, Guid societeId, Guid stagiaireId)
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

        private TestPlaceContext CreateTestPlace()
        {
            var stagiaireId = Guid.NewGuid();
            var societeId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            var builder = Aggregate.Make<Seat>().AddEvent(new SeatCreated(Guid.Empty, 1, sessionId, stagiaireId, societeId));

            return new TestPlaceContext(builder, sessionId, societeId, stagiaireId);
        }
    }
}