using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Utilisateurs;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;

namespace GestionFormation.CoreDomain.Rappels.Queries
{
    public class RappelSqlQueries : IRappelQueries
    {
        public IEnumerable<IRappelResult> GetAll(UtilisateurRole role)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Rappels.Where(a => a.AffectedRole == role).ToList().Select(a => new RappelResult(a));
            }
        }
    }
}