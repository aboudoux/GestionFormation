using System;
using System.Collections.Generic;

namespace GestionFormation.Kernel
{  
    public class EventPlayer
    {
        private readonly Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

        public EventPlayer Add<T>(Action<T> apply)
        {
            var type = typeof(T);
            if (_handlers.ContainsKey(type))
                throw new ArgumentException($"Le type {type} a déjà été enregistré dans l'event player");
            _handlers.Add(type, a => apply((T)a) );

            return this;
        }

        public void Apply<T>(T @event) where T : IDomainEvent
        {
            var type = @event.GetType();

            if (_handlers.ContainsKey(type))
                _handlers[type](@event);
        }
    }
}