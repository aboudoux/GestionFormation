using System.Collections.Generic;

namespace GestionFormation.EventStore.Queries
{
    public interface IEventQueries
    {
        IEnumerable<IEventResult> GetAll();
    }
}