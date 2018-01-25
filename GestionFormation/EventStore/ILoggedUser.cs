using System;
using GestionFormation.CoreDomain.Utilisateurs;

namespace GestionFormation.EventStore
{
    public interface ILoggedUser
    {
        Guid UserId { get; }
        string Login { get; }
        string Nom { get; }
        string Prenom { get; }
        UtilisateurRole Role { get; }
    }
}