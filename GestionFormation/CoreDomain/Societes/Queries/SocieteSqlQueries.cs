using System.Collections.Generic;
using System.Linq;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Societes.Queries
{
    public class SocieteSqlQueries : ISocieteQueries, IRuntimeDependency
    {
        public IEnumerable<ISocieteResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Societes.ToList().Select(a => new SocieteResult(a));
            }
        }
    }
}