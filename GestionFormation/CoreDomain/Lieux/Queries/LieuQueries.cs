using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Lieux.Queries
{
    public class LieuQueries : ILieuQueries, IRuntimeDependency
    {
        public IReadOnlyList<ILieuResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Lieux.Where(a=>a.Actif).ToList().Select(entity => new LieuResult(entity)).ToList();
            }
        }

        public Guid? GetLieu(string nom)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Lieux.FirstOrDefault(a => a.Nom == nom)?.Id;
            }
        }
    }
}