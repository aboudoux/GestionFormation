using System;

namespace GestionFormation.Kernel
{
    public interface IDomainEvent
    {
        Guid AggregateId { get; }
        int Sequence { get; }
    }
}