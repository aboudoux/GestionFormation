using System;
using GestionFormation.CoreDomain.Utilisateurs;

namespace GestionFormation.EventStore
{
    public class LocalApplicationUser : ILoggedUser
    {
        public LocalApplicationUser()
        {
            UserId = Guid.Empty;
            Login = "SYSTEM";
            Nom = "Application";
            Role = UtilisateurRole.Admin;
        }
        public Guid UserId { get; }
        public string Login { get; }
        public string Nom { get; }
        public string Prenom { get; }
        public UtilisateurRole Role { get; }
    }
}