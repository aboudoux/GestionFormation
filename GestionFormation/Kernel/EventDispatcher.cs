using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GestionFormation.Kernel
{
    public class EventDispatcher
    {
        private readonly Dictionary<Type, List<IEventHandler>> _handlers = new Dictionary<Type, List<IEventHandler>>();

        public void Register<T>(T handler) 
            where T : IEventHandler
        {
            var handlerType = handler.GetType();                           

            var interfaces = handlerType.GetInterfaces().Where(a => a.IsGenericType && a.GetGenericTypeDefinition() == typeof(IEventHandler<>)).ToList();
            foreach (var @interface in interfaces)
            {
                var eventType = @interface.GetGenericArguments().First();
                if (!_handlers.ContainsKey(eventType))
                    _handlers.Add(eventType, new List<IEventHandler>());
                _handlers[eventType].Add(handler);
            }
        }

        public void AutoRegisterAllEventHandler()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetAllConcretTypeThatImplementInterface<IEventHandler>())            
                Register(Activator.CreateInstance(type) as IEventHandler);            
        }

        public void AutoRegisterProjectionEventHandlerOnly()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetAllConcretTypeThatImplementInterface<IEventHandler>().Where(a=>a.GetInterface(typeof(IProjectionHandler).Name) != null))
                Register(Activator.CreateInstance(type) as IEventHandler);
        }

        public void Dispatch<T>(T @event) where T : IDomainEvent
        {
            var eventType = @event.GetType();
            if(!_handlers.ContainsKey(eventType))
                return;

            foreach (var handler in _handlers[eventType])
                typeof(IEventHandler<>).MakeGenericType(eventType).InvokeMember("Handle", BindingFlags.InvokeMethod, null, handler, new object[] {@event});
        }
    }
}