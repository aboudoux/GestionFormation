using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Utilisateurs
{
    public class UtilisateurRoleChanged : DomainEvent
    {
        public UtilisateurRole Role { get; }

        public UtilisateurRoleChanged(Guid aggregateId, int sequence, UtilisateurRole role) : base(aggregateId, sequence)
        {
            Role = role;
        }

        protected override string Description => "Utilisateur role changé";
    }
}