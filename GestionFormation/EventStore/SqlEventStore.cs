using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.Kernel;

namespace GestionFormation.EventStore
{
    public class SqlEventStore : IEventStore
    {
        private readonly IEventSerializer _eventSerializer;
        private IEventStamping _eventStamping;

        public SqlEventStore(IEventSerializer eventSerializer, IEventStamping eventStamping)
        {
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
            _eventStamping = eventStamping ?? throw new ArgumentNullException(nameof(eventStamping));
        }

        public void SetEventStamping(IEventStamping eventStamping)
        {
            _eventStamping = eventStamping ?? throw new ArgumentNullException(nameof(eventStamping));
        }

        public void Save(IDomainEvent @event)
        {
            using (var context = new EventStoreContext(ConnectionString.Get()))
            {
                var dbEvent = new DbEvent(@event, _eventSerializer, _eventStamping);
                context.Events.Add(dbEvent);
                context.SaveChanges();
            }
        }

        public int GetLastSequence(Guid aggregateId)
        {
            using (var context = new EventStoreContext(ConnectionString.Get()))
            {
                var maxSequence = context.Database.SqlQuery<int?>($"SELECT MAX(Sequence) FROM dbo.Event where AggregateId = '{aggregateId}'").FirstOrDefault();
                return maxSequence ?? 0;
            }
        }

        public IReadOnlyList<IDomainEvent> GetEvents(Guid aggregateId)
        {
            var events = new List<IDomainEvent>();
            using (var context = new EventStoreContext(ConnectionString.Get()))
            {
                foreach (var dbEvent in context.Events.Where(a => a.AggregateId == aggregateId))
                    events.Add(_eventSerializer.Deserialize<IDomainEvent>(dbEvent.Data));
            }

            return events;
        }

        public IReadOnlyList<IDomainEvent> GetAllEvents()
        {
            var events = new List<IDomainEvent>();
            using (var context = new EventStoreContext(ConnectionString.Get()))
            {
                foreach (var dbEvent in context.Events.Where(a=>!a.EventName.Contains("Test")))
                    events.Add(_eventSerializer.Deserialize<IDomainEvent>(dbEvent.Data));
            }

            return events;
        }
    }
}