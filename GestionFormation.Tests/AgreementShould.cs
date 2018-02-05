using System;
using System.Collections.Generic;
using FluentAssertions;
using GestionFormation.Applications.Agreements;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Agreements.Events;
using GestionFormation.CoreDomain.Agreements.Exceptions;
using GestionFormation.CoreDomain.Seats.Events;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using GestionFormation.Tests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class AgreementShould
    {
        [TestMethod]
        public void raise_conventionCreated_when_create_convention()
        {
            var contactId = Guid.NewGuid();
            var convention = Agreement.Create(contactId, 6000, AgreementType.Free);
            convention.UncommitedEvents.GetStream().Should().Contain(new AgreementCreated(Guid.Empty, 0, contactId, 6000, AgreementType.Free));
        }

        [TestMethod]
        public void raise_convention_revoked_whe_revoke_convention()
        {
            var context = CreateConvention();
            var convention = context.Builder.Create();
            convention.Revoke();
            convention.UncommitedEvents.GetStream().Should().Contain(new AgreementRevoked(Guid.Empty, 0));
        }

        [TestMethod]
        public void dont_raise_convention_revoked_twice()
        {
            var context = CreateConvention();
            context.Builder.AddEvent(new AgreementRevoked(Guid.NewGuid(), 2));
            var convention = context.Builder.Create();

            convention.Revoke();
            convention.UncommitedEvents.GetStream().Should().BeEmpty();
        }

        [TestMethod]
        public void raise_conventionSigned_when_upload_document()
        {
            var context = CreateConvention();
            var convention = context.Builder.Create();

            var documentId = Guid.NewGuid();
            convention.Sign(documentId);
            convention.UncommitedEvents.GetStream().Should().Contain(new AgreementSigned(Guid.Empty, 0, documentId));
        }

        [TestMethod]
        public void throw_error_if_sign_revoked_convention()
        {
            var context = CreateConvention();
            var convention = context.Builder.Create();

            convention.Revoke();
            Action action = () => convention.Sign(Guid.NewGuid());
            action.ShouldThrow<CannotSignRevokedAgreement>();
        }

        [TestMethod]
        public void throw_error_if_create_convention_with_not_same_societe()
        {
            var sessionId = Guid.NewGuid();
            var place1Id = Guid.NewGuid();
            var place2Id = Guid.NewGuid();

            var eventStore = new FakeEventStore();
            eventStore.Save(new SeatCreated(place1Id,1, sessionId, Guid.NewGuid(), Guid.NewGuid()));
            eventStore.Save(new SeatCreated(place2Id,1, sessionId, Guid.NewGuid(), Guid.NewGuid()));
            eventStore.Save(new SeatValided(place1Id, 2));
            eventStore.Save(new SeatValided(place2Id, 2));

            var createConvention = new CreateAgreement(new EventBus(new EventDispatcher(), eventStore), new FakeAgreementQueries());
            Action action = () => createConvention.Execute(Guid.NewGuid(), new List<Guid>() {place1Id, place2Id}, AgreementType.Free);

            action.ShouldThrow<AgreementCompanyException>();
        }

        [TestMethod]
        public void throw_error_if_create_convention_has_duplicate_()
        {
            var placeId = Guid.NewGuid();
            var createConvention = new CreateAgreement(new EventBus(new EventDispatcher(), new FakeEventStore()), new FakeAgreementQueries());
            Action action = () => createConvention.Execute(Guid.NewGuid(), new List<Guid>() { placeId, placeId},AgreementType.Free);

            action.ShouldThrow<ArgumentException>();
        }

        private class TestConventionContext
        {
            public Aggregate.AggregateBuilder<Agreement> Builder { get; }
            private Guid ContactId { get; }

            public TestConventionContext(Aggregate.AggregateBuilder<Agreement> builder, Guid contactId)
            {
                Builder = builder;
                ContactId = contactId;
            }            
        }

        private TestConventionContext CreateConvention()
        {
            var contactId = Guid.NewGuid();
            var builder = Aggregate.Make<Agreement>().AddEvent(new AgreementCreated(Guid.Empty, 1, contactId, 6000, AgreementType.Free));
            return new TestConventionContext(builder, contactId);
        }
    }
}