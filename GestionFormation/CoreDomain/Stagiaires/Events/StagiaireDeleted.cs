using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Stagiaires.Events
{
    public class StagiaireDeleted : DomainEvent
    {
        public StagiaireDeleted(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
            
        }

        protected override string Description => "Stagiaire supprimé";
    }
}