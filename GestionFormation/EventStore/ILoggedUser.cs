using System;
using GestionFormation.CoreDomain.Users;

namespace GestionFormation.EventStore
{
    public interface ILoggedUser
    {
        Guid UserId { get; }
        string Login { get; }
        string Nom { get; }
        string Prenom { get; }
        UserRole Role { get; }
        string Signature { get; }
    }
}