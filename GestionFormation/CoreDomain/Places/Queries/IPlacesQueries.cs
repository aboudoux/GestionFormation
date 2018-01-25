using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Places.Queries
{
    public interface IPlacesQueries
    {
        IEnumerable<IPlaceResult> GetAll(Guid sessionId);
        IEnumerable<IConventionPlaceResult> GetConventionPlaces(Guid conventionId);
        IEnumerable<IPlaceValidatedResult> GetValidatedPlaces(Guid sessionId);
    }
}