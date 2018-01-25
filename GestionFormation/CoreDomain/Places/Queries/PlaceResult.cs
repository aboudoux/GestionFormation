using System;
using GestionFormation.CoreDomain.Conventions;
using GestionFormation.CoreDomain.Conventions.Projections;
using GestionFormation.CoreDomain.Places.Projections;

namespace GestionFormation.CoreDomain.Places.Queries
{
    public class PlaceResult : IPlaceResult
    {
        public PlaceResult(PlaceSqlentity a, ConventionSqlEntity convention)
        {
            PlaceId = a.PlaceId;
            StagiaireId = a.StagiaireId;
            SocieteId = a.SocieteId;
            Status = a.Status;
            Raison = a.Raison;
            ConventionId = a.AssociatedConventionId;

            if (convention != null)
            {
                NumeroConvention = convention.ConventionNumber;
                ConventionSigned = convention.DocumentId.HasValue;
                TypeConvention = convention.TypeConvention;
            }
        }

        public Guid PlaceId { get; }
        public Guid StagiaireId { get; }
        public Guid SocieteId { get; }
        public PlaceStatus Status { get; }
        public string Raison { get; }
        public Guid? ConventionId { get; }
        public string NumeroConvention { get; }
        public bool ConventionSigned { get; }
        public TypeConvention TypeConvention { get; }
    }
}