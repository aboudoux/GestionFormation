using System;
using System.Collections.Generic;

namespace GestionFormation.Kernel
{
    public interface IEventStore
    {
        void Save(IDomainEvent @event);

        int GetLastSequence(Guid aggregateId);

        IReadOnlyList<IDomainEvent> GetEvents(Guid aggregateId);
        IReadOnlyList<IDomainEvent> GetAllEvents();
    }
}