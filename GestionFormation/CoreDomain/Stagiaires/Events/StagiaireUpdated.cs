using System;

namespace GestionFormation.CoreDomain.Stagiaires.Events
{
    public class StagiaireUpdated : StagiaireCreated
    {
        public StagiaireUpdated(Guid aggregateId, int sequence, string nom, string prenom) : base(aggregateId, sequence, nom, prenom)
        {          
        }
    }
}