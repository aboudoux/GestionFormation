using System.Collections.Generic;
using GestionFormation.Kernel;

namespace GestionFormation.Tests.Fakes
{
    public class MockHandler<T> : IEventHandler<T> where T : IDomainEvent
    {
        public List<IDomainEvent> AllEvents = new List<IDomainEvent>();

        public void Handle(T @event)
        {
            AllEvents.Add(@event);
        }
    }

    public class MockHandler<T1, T2> : MockHandler<T1>, IEventHandler<T2>
        where T1 : IDomainEvent
        where T2 : IDomainEvent
    {
        public void Handle(T2 @event)
        {
            AllEvents.Add(@event);
        }
    }

    public class MockHandler<T1, T2, T3> : MockHandler<T1, T2>, IEventHandler<T3>
        where T1 : IDomainEvent
        where T2 : IDomainEvent
        where T3 : IDomainEvent
    {
        public void Handle(T3 @event)
        {
            AllEvents.Add(@event);
        }
    }
}