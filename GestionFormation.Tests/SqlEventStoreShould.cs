using System;
using System.Reflection;
using FluentAssertions;
using GestionFormation.EventStore;
using GestionFormation.Kernel;
using GestionFormation.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests
{
    [TestClass]
    [TestCategory("IntegrationTests")]
    public class SqlEventStoreShould
    {
        [TestMethod]
        public void create_db_event_from_event()
        {
            var id = Guid.Parse("A761A9C5-DC6A-4CF2-A75C-228D64553BA7");            
            var dbEvent = new DbEvent(new SqlTestEvent(id, 1, "ESSAI"), new DomainEventJsonEventSerializer(), new FakeEventStamping());

            dbEvent.AggregateId.Should().Be(id);
            dbEvent.EventName.Should().Be(nameof(SqlTestEvent));
            dbEvent.Sequence.Should().Be(1);
            dbEvent.Data.Should().Be(@"{""$type"":""SqlTestEvent"",""Label"":""ESSAI"",""AggregateId"":""a761a9c5-dc6a-4cf2-a75c-228d64553ba7"",""Sequence"":1}");
            dbEvent.TimeStamp.Should().BeBefore(DateTime.Now);
        }

        [TestMethod]
        public void save_event_in_database()
        {
            var store = new SqlEventStore(new DomainEventJsonEventSerializer(), new FakeEventStamping());
            store.Save(new SqlTestEvent(Guid.NewGuid(), 1, "test"));
        }

        [TestMethod]
        public void get_max_sequence_if_no_corresponding_event()
        {
            var store = new SqlEventStore(new DomainEventJsonEventSerializer(), new FakeEventStamping());
            var sequence = store.GetLastSequence(Guid.NewGuid());
            sequence.Should().Be(0);
        }

        [TestMethod]
        public void get_max_sequence_if_events_exists()
        {
            var id = Guid.NewGuid();
            var store = new SqlEventStore(new DomainEventJsonEventSerializer(), new FakeEventStamping());
            store.Save(new SqlTestEvent(id, 1, "test"));
            store.Save(new SqlTestEvent(id, 2, "test2"));
            store.Save(new SqlTestEvent(id, 3, "test3"));
            var lastSequence = store.GetLastSequence(id);
            lastSequence.Should().Be(3);
        }

        [TestMethod]
        public void retrieve_events_for_registered_aggregate()
        {
            var id = Guid.NewGuid();
            var store = new SqlEventStore(new DomainEventJsonEventSerializer(new DomainEventTypeBinder(Assembly.GetExecutingAssembly())), new FakeEventStamping());
            store.Save(new SqlTestEvent(id, 1, "test"));
            store.Save(new SqlTestEvent(id, 2, "test2"));
            store.Save(new SqlTestEvent(id, 3, "test3"));

            var events = store.GetEvents(id);
            events.Should().HaveCount(3);
            events.Should().Contain(new SqlTestEvent(id, 1, "test"));
        }
    }

    public class SqlTestEvent : DomainEvent
    {
        public SqlTestEvent(Guid aggregateId, int sequence, string label) : base(aggregateId, sequence)
        {
            Label = label;
        }

        public string Label { get; }
        protected override string Description { get; }
    }
}