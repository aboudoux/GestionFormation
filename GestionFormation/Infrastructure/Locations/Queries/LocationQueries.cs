using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Locations.Queries;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure.Locations.Queries
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

        public Guid? GetLocation(string name)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Locations.FirstOrDefault(a => a.Enabled && a.Name == name)?.Id;
            }
        }
    }
}