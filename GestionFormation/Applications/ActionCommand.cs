using System;
using System.Linq;
using GestionFormation.Kernel;

namespace GestionFormation.Applications
{
    public abstract class ActionCommand
    {
        protected readonly EventBus EventBus;

        protected ActionCommand(EventBus eventBus)
        {
            EventBus = eventBus;
        }

        protected T GetAggregate<T>(Guid aggregateId, bool throwExceptionIfNotExists = false)
        {
            var loadedEvents = EventBus.GetEventStore().GetEvents(aggregateId);
            if (loadedEvents == null || !loadedEvents.Any())
            {        
                if(throwExceptionIfNotExists)
                    throw new AggregateNotFoundException(typeof(T), aggregateId);
                return default(T);
            }

            var history = new History(loadedEvents);
            var aggregate = Activator.CreateInstance(typeof(T), history);
            return (T)aggregate;
        }

        protected void PublishUncommitedEvents(params AggregateRoot[] aggregates)
        {
            foreach (var aggregate in aggregates)
            {
                if(aggregate != null)
                    EventBus.Publish(aggregate.UncommitedEvents);
            }            
        }
    }
}