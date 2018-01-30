using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Locations.Queries
{
    public class LocationQueries : ILocationQueries, IRuntimeDependency
    {
        public IReadOnlyList<ILocationResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Locations.Where(a=>a.Enabled).ToList().Select(entity => new LocationResult(entity)).ToList();
            }
        }

        public Guid? GetLocation(string nom)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Locations.FirstOrDefault(a => a.Name == nom)?.Id;
            }
        }
    }
}