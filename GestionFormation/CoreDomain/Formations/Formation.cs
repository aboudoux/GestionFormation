using System;
using GestionFormation.CoreDomain.Formations.Events;
using GestionFormation.CoreDomain.Formations.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formations
{
    public class Formation : AggregateRootUpdatableAndDeletable<FormationUpdated, FormationDeleted>
    {
        public Formation(History history) : base(history)
        {
        }
     
        public static Formation Create(string nom, int places)
        {
            if(string.IsNullOrEmpty(nom))
                throw new FormationEmptyNameException();

            var formation = new Formation(new History());
            formation.AggregateId = Guid.NewGuid();
            formation.UncommitedEvents.Add(new FormationCreated(formation.AggregateId, 1, nom, places));
            return formation;
        }

        public void Update(string nom, int places)
        {
            if (string.IsNullOrEmpty(nom))
                throw new FormationEmptyNameException();

            Update(new FormationUpdated(AggregateId, GetNextSequence(), nom, places));
        }

        public void Delete()
        {
            Delete(new FormationDeleted(AggregateId, GetNextSequence()));            
        }
    }
}
