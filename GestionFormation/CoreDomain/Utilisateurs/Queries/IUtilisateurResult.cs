using System;

namespace GestionFormation.CoreDomain.Utilisateurs.Queries
{
    public interface IUtilisateurResult
    {
        Guid Id { get; }
        string Login { get; }
        string Nom { get; }
        string Prenom { get; }
        bool IsEnabled { get; }
        string Email { get; }
        UtilisateurRole Role { get; }
    }
}