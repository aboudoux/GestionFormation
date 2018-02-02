using System;
using GestionFormation.CoreDomain.Users;

namespace GestionFormation.EventStore
{
    public class LoggedUser: ILoggedUser
    {
        public LoggedUser(Guid userId, string login, string nom, string prenom, UserRole role)
        {
            if(userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

            UserId = userId;
            Login = login;
            Nom = nom;
            Prenom = prenom;
            Role = role;
        }
        public Guid UserId { get; }
        public string Login { get; }
        public string Nom { get; }
        public string Prenom { get; }
        public UserRole Role { get; }

        public override string ToString()
        {
            return $"{Nom} {Prenom} ({Login})";
        }
    }
}