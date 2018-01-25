using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Utilisateurs
{
    public class PasswordChanged : DomainEvent
    {
        public string EncryptedPassword { get; }

        public PasswordChanged(Guid aggregateId, int sequence, string encryptedPassword) : base(aggregateId, sequence)
        {
            EncryptedPassword = encryptedPassword;
        }
    }
}