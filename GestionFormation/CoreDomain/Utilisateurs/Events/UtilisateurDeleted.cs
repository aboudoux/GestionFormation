using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Utilisateurs
{
    public class UtilisateurDeleted : DomainEvent
    {
        public UtilisateurDeleted(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }
    }
}