using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GestionFormation.Kernel;

namespace GestionFormation.Tests.Tools
{
    public static class Aggregate
    {
        public static AggregateBuilder<T> Make<T>() where T : AggregateRoot
        {
            return new AggregateBuilder<T>();
        }

        public class AggregateBuilder<TAggregate> where TAggregate : AggregateRoot
        {
            private readonly History _history = new History();

            public AggregateBuilder<TAggregate> AddEvent<T>(T @event) where T : DomainEvent
            {
                _history.Add(@event);
                return this;
            }

            public TAggregate Create()
            {
                return Activator.CreateInstance(typeof(TAggregate), _history) as TAggregate;
            }
        }
    }
}
