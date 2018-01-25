using System;
using FluentAssertions;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("UnitTests")]
    public class EventBusShould
    {        
        [TestMethod]
        public void throw_error_if_sequence_not_consistent_with_event_store()
        {
            var store = new FakeEventStore();
            var eventBus = new EventBus(new EventDispatcher(), store);

            var uncommitedEvent = new UncommitedEvents();

            var guid = Guid.NewGuid();
            uncommitedEvent.Add(new TestDomainEvent(guid, 1));
            uncommitedEvent.Add(new TestDomainEvent(guid, 2));
            uncommitedEvent.Add(new TestDomainEvent(guid, 3));
            eventBus.Publish(uncommitedEvent);

            uncommitedEvent.Add(new TestDomainEvent(guid, 3));
            var publish = (Action)(() => eventBus.Publish(uncommitedEvent));

            publish.ShouldThrow<ConsistencyException>();
        }
    }
}