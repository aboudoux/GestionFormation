using System;
using GestionFormation.CoreDomain.Places.Projections;

namespace GestionFormation.CoreDomain.Places.Queries
{
    public interface IPlaceResult
    {
        Guid PlaceId { get; }
        Guid StagiaireId { get; }
        Guid SocieteId { get; }
        PlaceStatus Status { get; }
        string Raison { get; }
        Guid? ConventionId { get; }
        string NumeroConvention { get; }
        bool ConventionSigned { get; }
    }
}