using System;
using GestionFormation.Kernel;

namespace GestionFormation.Tests.Fakes
{
    public class TestDomainEvent : IDomainEvent
    {
        public TestDomainEvent(Guid aggregateId, int sequence)
        {
            AggregateId = aggregateId;
            Sequence = sequence;
        }

        public Guid AggregateId { get; }
        public int Sequence { get; }
    }
}