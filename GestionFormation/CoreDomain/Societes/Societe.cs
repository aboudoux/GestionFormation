using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Societes
{
    public class Societe : AggregateRootUpdatableAndDeletable<SocieteUpdated, SocieteDeleted>
    {
        public Societe(History history) : base(history)
        {
        }

        public static Societe Create(string nom, string addresse, string codepostal, string ville)
        {
            var societe = new Societe(History.Empty);
            societe.AggregateId = Guid.NewGuid();
            societe.UncommitedEvents.Add(new SocieteCreated(societe.AggregateId, 1, nom, addresse, codepostal, ville ));
            return societe;
        }

        public void Update(string nom, string addresse, string codepostal, string ville)
        {
            Update(new SocieteUpdated(AggregateId, GetNextSequence(), nom, addresse, codepostal, ville));
        }

        public void Delete()
        {
            Delete(new SocieteDeleted(AggregateId, GetNextSequence()));
        }
    }
}
