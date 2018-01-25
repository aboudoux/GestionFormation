using System;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.Applications
{
    public class EventReplayer
    {
        private readonly IEventStore _eventStore;
        private readonly EventDispatcher _dispatcher;

        public EventReplayer(IEventStore eventStore)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));

            _dispatcher = new EventDispatcher();
            _dispatcher.AutoRegisterProjectionEventHandler();
        }

        public void Execute(Action<int, int> progession = null)
        {
            var allEvents = _eventStore.GetAllEvents();
            var currentEventIndex = 1;
            foreach (var domainEvent in allEvents)
            {
                progession?.Invoke(currentEventIndex++,allEvents.Count);                
                _dispatcher.Dispatch(domainEvent);
                
            }
        }
    }
}