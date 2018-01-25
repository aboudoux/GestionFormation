using System;
using GestionFormation.CoreDomain.Rappels.Projections;

namespace GestionFormation.CoreDomain.Rappels.Queries
{
    public class RappelResult : IRappelResult
    {
        public RappelResult(RappelSqlEntity entity)
        {
            Label = entity.Label;
            AggregateId = entity.AggregateId;
            AggregateType = entity.AggregateType;
            RappelType = entity.RappelType;
        }
        public string Label { get; }
        public Guid AggregateId { get;  }
        public string AggregateType { get; }
        public RappelType RappelType { get; }
    }
}