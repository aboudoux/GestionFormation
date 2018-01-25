using System;
using GestionFormation.CoreDomain.Rappels.Projections;

namespace GestionFormation.CoreDomain.Rappels.Queries
{
    public interface IRappelResult
    {
        string Label { get; }
        Guid AggregateId { get; }
        string AggregateType { get;  }
        RappelType RappelType { get; }

    }
}