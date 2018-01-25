using System;

namespace GestionFormation.CoreDomain.Formations.Queries
{
    public interface IFormationResult
    {
        Guid Id { get; }
        string Nom { get; }
        int Places { get; }
    }
}