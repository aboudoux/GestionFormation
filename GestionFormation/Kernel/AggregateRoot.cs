using System;

namespace GestionFormation.Kernel
{
    public abstract class AggregateRoot
    {
        private readonly EventPlayer _player = new EventPlayer();
        public Guid AggregateId { get; protected set; }
        private int _lastSequenceNumber;

        public UncommitedEvents UncommitedEvents = new UncommitedEvents();

        protected int GetNextSequence()
        {
            return ++_lastSequenceNumber;
        }

        protected AggregateRoot(History history)
        {
            if (history == null) throw new ArgumentNullException(nameof(history));
            AddPlayers(_player);
            HydrateFrom(history);
        }

        protected abstract void AddPlayers(EventPlayer player);

        protected void Apply<T>(T @event) where T : IDomainEvent
        {
            _player.Apply(@event);
        }

        protected void HydrateFrom(History history)
        {
            foreach (var domainEvent in history.GetStream())
            {
                AggregateId = domainEvent.AggregateId;
                _lastSequenceNumber = domainEvent.Sequence;
                _player.Apply(domainEvent);
            }
        }

        protected void RaiseEvent(IDomainEvent @event)
        {
            UncommitedEvents.Add(@event);
            Apply(@event);
        }
    }
}