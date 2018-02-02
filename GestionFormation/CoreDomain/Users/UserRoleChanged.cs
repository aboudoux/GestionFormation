using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Users
{
    public class UserRoleChanged : DomainEvent
    {
        public UserRole Role { get; }

        public UserRoleChanged(Guid aggregateId, int sequence, UserRole role) : base(aggregateId, sequence)
        {
            Role = role;
        }

        protected override string Description => "Utilisateur role changé";
    }
}