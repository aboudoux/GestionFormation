using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Conventions.Queries
{
    public interface IConventionResult
    {
        Guid ConventionId { get; }
        string Societe { get; }
        string Contact { get; }
        List<Guid> Places { get; }
        string ConventionNumber { get; }

    }
}