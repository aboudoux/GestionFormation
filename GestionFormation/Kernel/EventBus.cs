using System;

namespace GestionFormation.Kernel
{
    public class EventBus
    {
        private readonly IEventStore _eventStore;
        private readonly EventDispatcher _dispatcher;

        public EventBus(EventDispatcher dispatcher, IEventStore eventStore)
        {
            _eventStore = eventStore;
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public void Publish(UncommitedEvents events)
        {
            foreach (var eEvent in events.GetStream())
            {
                if(_eventStore.GetLastSequence(eEvent.AggregateId) >= eEvent.Sequence)
                    throw new ConsistencyException(eEvent);

                _eventStore.Save(eEvent);
                _dispatcher.Dispatch(eEvent);
            }
        }

        public IEventStore GetEventStore()
        {
            return _eventStore;
        }
    }
}