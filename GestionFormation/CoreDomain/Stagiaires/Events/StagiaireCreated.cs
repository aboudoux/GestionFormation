using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Stagiaires.Events
{
    public class StagiaireCreated : DomainEvent
    {
        public string Nom { get; }
        public string Prenom { get; }

        public StagiaireCreated(Guid aggregateId, int sequence, string nom, string prenom) : base(aggregateId, sequence)
        {
            Nom = nom;
            Prenom = prenom;            
        }
    }
}