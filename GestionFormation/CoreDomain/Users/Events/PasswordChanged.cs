using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Users.Events
{
    public class PasswordChanged : DomainEvent
    {
        public string EncryptedPassword { get; }

        public PasswordChanged(Guid aggregateId, int sequence, string encryptedPassword) : base(aggregateId, sequence)
        {
            EncryptedPassword = encryptedPassword;
        }

        protected override string Description => "Mot de passe changé";
    }
}