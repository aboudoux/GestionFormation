using System.Collections.Generic;
using System.Linq;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formateurs.Queries
{
    public class FormateurSqlQueries : IFormateurQueries, IRuntimeDependency
    {
        public IReadOnlyList<IFormateurResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Formateurs.ToList().Select(a=>new FormateurResult(a)).ToList();
            }
        }
    }
}