using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formations.Events
{
    public class FormationCreated : DomainEvent
    {
        public string Nom { get; }
        public int Places { get; }

        public FormationCreated(Guid aggregateId, int sequence, string nom, int places) : base(aggregateId, sequence)
        {
            Nom = nom;
            Places = places;
        }

        protected override string Description => "Formation créé";
    }
}