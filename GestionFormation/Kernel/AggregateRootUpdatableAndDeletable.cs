using System;

namespace GestionFormation.Kernel
{
    public class AggregateRootUpdatableAndDeletable<TUpdateEvent, TDeleteEvent> : AggregateRoot
        where TUpdateEvent : IDomainEvent
        where TDeleteEvent : IDomainEvent        
    {
        protected bool _isDeleted;
        protected TUpdateEvent _lastUpdate;

        public AggregateRootUpdatableAndDeletable(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));
            player.Add<TDeleteEvent>(e => _isDeleted = true)
                .Add<TUpdateEvent>(e => _lastUpdate = e);
        }      

        protected void Update(TUpdateEvent updateEvent)
        {
            var @event = updateEvent;

            if (_lastUpdate != null && @event.Equals(_lastUpdate))
                return;

            UncommitedEvents.Add(@event);
            Apply(@event);
        }

        protected void Delete(TDeleteEvent deleteEvent)
        {
            if (_isDeleted)
                return;

            var @event = deleteEvent;
            UncommitedEvents.Add(@event);
            Apply(@event);
        }
    }
}