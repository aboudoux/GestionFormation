using System;
using GestionFormation.CoreDomain.Rappels.Projections;

namespace GestionFormation.CoreDomain.Rappels.Queries
{
    public interface IRappelResult
    {
        string Label { get; }
        
        RappelType RappelType { get; }

        Guid? PlaceId { get; set; }
        Guid? SessionId { get; set; }
        Guid? SocieteId { get; set; }
        Guid? ConventionId { get; set; }
    }
}