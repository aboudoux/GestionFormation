using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Locations.Queries
{
    public interface ILocationQueries
    {
        IReadOnlyList<ILocationResult> GetAll();

        Guid? GetLocation(string name);
    }
}