using System;
using GestionFormation.CoreDomain.Rappels.Projections;

namespace GestionFormation.CoreDomain.Rappels.Queries
{
    public class RappelResult : IRappelResult
    {
        public RappelResult(RappelSqlEntity entity)
        {
            Label = entity.Label;
            RappelType = entity.RappelType;
            PlaceId = entity.PlaceId;
            SessionId = entity.SessionId;
            SessionId = entity.SessionId;
            ConventionId = entity.ConventionId;
        }

        public string Label { get; }
        public RappelType RappelType { get; }
        public Guid? PlaceId { get; set; }
        public Guid? SessionId { get; set; }
        public Guid? SocieteId { get; set; }
        public Guid? ConventionId { get; set; }
    }
}