using System;

namespace GestionFormation.Kernel
{
    public class ConsistencyException : Exception
    {
        public ConsistencyException(IDomainEvent @event) : base($"the event of type {@event.GetType().Name} with id {@event.AggregateId} as an inconsistency sequence number {@event.Sequence}")
        {
            
        }
    }
}