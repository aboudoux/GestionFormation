using System;

namespace GestionFormation.CoreDomain.Locations.Queries
{
    public interface ILocationResult
    {
        Guid LocationId { get; }
        string Name { get; }
        string Address { get; }
        int Seats { get; }
    }
}
