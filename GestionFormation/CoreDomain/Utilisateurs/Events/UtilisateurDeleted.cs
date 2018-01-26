using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Utilisateurs.Events
{
    public class UtilisateurDeleted : DomainEvent
    {
        public UtilisateurDeleted(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }

        protected override string Description => "Utilisateur supprimé";
    }
}