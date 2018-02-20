using System;
using GestionFormation.CoreDomain.Locations.Queries;
using GestionFormation.Infrastructure.Locations.Projections;

namespace GestionFormation.Infrastructure.Locations.Queries
{
    public class LocationResult : ILocationResult
    {
        public LocationResult(LocationSqlEntity entity)
        {
            LocationId = entity.Id;
            Name = entity.Name;
            Address = entity.Address;
            Seats = entity.Seats;
        }
        public Guid LocationId { get; }
        public string Name { get; }
        public string Address { get; }
        public int Seats { get; }
    }
}