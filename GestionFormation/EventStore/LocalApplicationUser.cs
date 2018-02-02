using System;
using GestionFormation.CoreDomain.Users;

namespace GestionFormation.EventStore
{
    public class LocalApplicationUser : ILoggedUser
    {
        public LocalApplicationUser()
        {
            UserId = Guid.Empty;
            Login = "SYSTEM";
            Nom = "Application";
            Role = UserRole.Admin;
        }
        public Guid UserId { get; }
        public string Login { get; }
        public string Nom { get; }
        public string Prenom { get; }
        public UserRole Role { get; }
    }
}