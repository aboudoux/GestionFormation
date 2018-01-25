using System;

namespace GestionFormation.CoreDomain.Societes.Queries
{
    public interface ISocieteResult
    {
        Guid SocieteId { get; }
        string Nom { get; }
        string Adresse { get; }
        string Codepostal { get; }
        string Ville { get; }
    }
}