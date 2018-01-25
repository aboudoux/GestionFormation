using System;

namespace GestionFormation.CoreDomain.Stagiaires.Queries
{
    public interface IStagiaireResult
    {
        Guid Id { get; }
        string Nom { get; }
        string Prenom { get; }
    }
}