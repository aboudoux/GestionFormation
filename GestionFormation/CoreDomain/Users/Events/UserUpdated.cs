using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Users.Events
{
    public class UserUpdated : DomainEvent
    {
        public string Lastname { get; }
        public string Firstname { get; }
        public string Email { get; }
        public bool IsEnabled { get; }
        public string Signature { get; }

        public UserUpdated(Guid aggregateId, int sequence, string lastname, string firstname, string email, bool isEnabled, string signature) : base(aggregateId, sequence)
        {
            Lastname = lastname;
            Firstname = firstname;
            Email = email;
            IsEnabled = isEnabled;
            Signature = signature;
        }

        protected override string Description => "Utilisateur modifié";
    }
}