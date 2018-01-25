using System.Collections.Generic;
using System.Linq;

namespace GestionFormation.Kernel
{
    public abstract class EventStream
    {
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();
        

        public IOrderedEnumerable<IDomainEvent> GetStream()
        {
            return _events.OrderBy(a=>a.Sequence);
        }

        public void Add(IDomainEvent @event)
        {
            _events.Add(@event);
        }
    }
}