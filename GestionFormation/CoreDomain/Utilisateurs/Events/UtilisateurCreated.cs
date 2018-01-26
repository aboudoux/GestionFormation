using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Utilisateurs.Events
{
    public class UtilisateurCreated : DomainEvent
    {
        public string Login { get; }
        public string EncryptedPassword { get; }
        public string Nom { get; }
        public string Prenom { get; }
        public string Email { get; }
        public UtilisateurRole Role { get; }

        public UtilisateurCreated(Guid aggregateId, int sequence, string login, string encryptedPassword, string nom, string prenom, string email, UtilisateurRole role) : base(aggregateId, sequence)
        {
            Login = login;
            EncryptedPassword = encryptedPassword;
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Role = role;
        }

        protected override string Description => "Utilisateur créé";
    }
}