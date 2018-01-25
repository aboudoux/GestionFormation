using System;

namespace GestionFormation.CoreDomain.Formateurs.Queries
{
    public interface IFormateurResult
    {
        Guid Id { get; }
        string Nom { get; }
        string Prenom { get; }
        string Email { get; }
    }
}