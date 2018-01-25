using System;
using GestionFormation.CoreDomain.Stagiaires.Events;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Stagiaires
{
    public class Stagiaire : AggregateRootUpdatableAndDeletable<StagiaireUpdated, StagiaireDeleted>
    {
        
        public Stagiaire(History history) : base(history)
        {
        }        

        public static Stagiaire Create(string nom, string prenom)
        {   
            var stagiaire = new Stagiaire(History.Empty);
            stagiaire.AggregateId = Guid.NewGuid();
            stagiaire.UncommitedEvents.Add(new StagiaireCreated(stagiaire.AggregateId, 1, nom, prenom));
            return stagiaire;

        }

        public void Update(string nom, string prenom)
        {
            Update(new StagiaireUpdated(AggregateId, GetNextSequence(), nom, prenom));
        }

        public void Delete()
        {
            Delete(new StagiaireDeleted(AggregateId, GetNextSequence()));          
        }
    }
}