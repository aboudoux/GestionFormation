using System;
using System.Collections.Generic;
using FluentAssertions;
using GestionFormation.Applications.Conventions;
using GestionFormation.CoreDomain.Conventions;
using GestionFormation.CoreDomain.Conventions.Events;
using GestionFormation.CoreDomain.Conventions.Exceptions;
using GestionFormation.CoreDomain.Places.Events;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using GestionFormation.Tests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class ConventionShould
    {
        [TestMethod]
        public void raise_conventionCreated_when_create_convention()
        {
            var contactId = Guid.NewGuid();
            var convention = Convention.Create(contactId, 6000);
            convention.UncommitedEvents.GetStream().Should().Contain(new ConventionCreated(Guid.Empty, 0, contactId, 6000));
        }

        [TestMethod]
        public void raise_convention_revoked_whe_revoke_convention()
        {
            var context = CreateConvention();
            var convention = context.Builder.Create();
            convention.Revoke();
            convention.UncommitedEvents.GetStream().Should().Contain(new ConventionRevoked(Guid.Empty, 0));
        }

        [TestMethod]
        public void dont_raise_convention_revoked_twice()
        {
            var context = CreateConvention();
            context.Builder.AddEvent(new ConventionRevoked(Guid.NewGuid(), 2));
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
            convention.UncommitedEvents.GetStream().Should().Contain(new ConventionSigned(Guid.Empty, 0, documentId));
        }

        [TestMethod]
        public void throw_error_if_sign_revoked_convention()
        {
            var context = CreateConvention();
            var convention = context.Builder.Create();

            convention.Revoke();
            Action action = () => convention.Sign(Guid.NewGuid());
            action.ShouldThrow<CannotSignRevokedConvention>();
        }

        [TestMethod]
        public void throw_error_if_create_convention_with_not_same_societe()
        {
            var sessionId = Guid.NewGuid();
            var place1Id = Guid.NewGuid();
            var place2Id = Guid.NewGuid();

            var eventStore = new FakeEventStore();
            eventStore.Save(new PlaceCreated(place1Id,1, sessionId, Guid.NewGuid(), Guid.NewGuid()));
            eventStore.Save(new PlaceCreated(place2Id,1, sessionId, Guid.NewGuid(), Guid.NewGuid()));
            eventStore.Save(new PlaceValided(place1Id, 2));
            eventStore.Save(new PlaceValided(place2Id, 2));

            var createConvention = new CreateConvention(new EventBus(new EventDispatcher(), eventStore), new FakeConventionQueries());
            Action action = () => createConvention.Execute(Guid.NewGuid(), new List<Guid>() {place1Id, place2Id});

            action.ShouldThrow<ConventionSocieteException>();
        }

        [TestMethod]
        public void throw_error_if_create_convention_has_duplicate_()
        {
            var placeId = Guid.NewGuid();
            var createConvention = new CreateConvention(new EventBus(new EventDispatcher(), new FakeEventStore()), new FakeConventionQueries());
            Action action = () => createConvention.Execute(Guid.NewGuid(), new List<Guid>() { placeId, placeId});

            action.ShouldThrow<ArgumentException>();
        }

        private class TestConventionContext
        {
            public Aggregate.AggregateBuilder<Convention> Builder { get; }
            private Guid ContactId { get; }

            public TestConventionContext(Aggregate.AggregateBuilder<Convention> builder, Guid contactId)
            {
                Builder = builder;
                ContactId = contactId;
            }            
        }

        private TestConventionContext CreateConvention()
        {
            var contactId = Guid.NewGuid();
            var builder = Aggregate.Make<Convention>().AddEvent(new ConventionCreated(Guid.Empty, 1, contactId, 6000));
            return new TestConventionContext(builder, contactId);
        }
    }
}