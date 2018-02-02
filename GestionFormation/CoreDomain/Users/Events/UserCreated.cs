using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Users.Events
{
    public class UserCreated : DomainEvent
    {
        public string Login { get; }
        public string EncryptedPassword { get; }
        public string Lastname { get; }
        public string Firstname { get; }
        public string Email { get; }
        public UserRole Role { get; }

        public UserCreated(Guid aggregateId, int sequence, string login, string encryptedPassword, string lastname, string firstname, string email, UserRole role) : base(aggregateId, sequence)
        {
            Login = login;
            EncryptedPassword = encryptedPassword;
            Lastname = lastname;
            Firstname = firstname;
            Email = email;
            Role = role;
        }

        protected override string Description => "Utilisateur créé";
    }
}