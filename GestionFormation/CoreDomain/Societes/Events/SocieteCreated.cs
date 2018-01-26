using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Societes
{
    public class SocieteCreated : DomainEvent
    {
        public string Nom { get; }
        public string Addresse { get; }
        public string Codepostal { get; }
        public string Ville { get; }

        public SocieteCreated(Guid aggregateId, int sequence, string nom, string addresse, string codepostal, string ville) : base(aggregateId, sequence)
        {
            Nom = nom;
            Addresse = addresse;
            Codepostal = codepostal;
            Ville = ville;
        }

        protected override string Description => "Société créée";
    }
}