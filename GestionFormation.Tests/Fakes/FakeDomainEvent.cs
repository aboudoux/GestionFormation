using System;
using GestionFormation.Kernel;

namespace GestionFormation.Tests.Fakes
{
    public abstract class FakeDomainEvent : IDomainEvent
    {
        public FakeDomainEvent()
        {
            AggregateId = Guid.Empty;
            Sequence = 0;
        }
        public Guid AggregateId { get; }
        public int Sequence { get; }
    }
}