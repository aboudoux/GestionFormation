using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.Kernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GestionFormation.Tests.Fakes
{
    public class FakeEventStore : IEventStore
    {
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();

        public void Save(IDomainEvent @event)
        {
            _events.Add(@event);            
        }

        public int GetLastSequence(Guid aggregateId)
        {
            var eventList = _events.Where(a => a.AggregateId == aggregateId).ToList();
            if (!eventList.Any())
                return 0;
            return eventList.Max(a => a.Sequence);
        }

        public IReadOnlyList<IDomainEvent> GetEvents(Guid aggregateId)
        {
            return _events.Where(a => a.AggregateId == aggregateId).ToList();
        }

        public IReadOnlyList<IDomainEvent> GetAllEvents()
        {
            throw new NotImplementedException();
        }
    }
}