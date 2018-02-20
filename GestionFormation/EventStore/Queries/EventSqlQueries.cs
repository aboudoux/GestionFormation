using System.Collections.Generic;
using System.Linq;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.EventStore.Queries
{
    public class EventSqlQueries : IEventQueries, IRuntimeDependency
    {        
        public IEnumerable<IEventResult> GetAll()
        {
            using (var context = new EventStoreContext(ConnectionString.Get()))
            {
                return context.Events.ToList().Select(a => new EventResult(a));
            }
        }
    }
}