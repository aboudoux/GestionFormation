using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Lieux.Events
{
    public class LieuUpdated : DomainEvent
    {
        public string Nom { get; }
        public string Addresse { get; }
        public int Places { get; }

        public LieuUpdated(Guid aggregateId, int sequence, string nom, string addresse, int places) : base(aggregateId, sequence)
        {
            Nom = nom;
            Addresse = addresse;
            Places = places;
        }

        protected override string Description => "Lieu modifié";
    }
}