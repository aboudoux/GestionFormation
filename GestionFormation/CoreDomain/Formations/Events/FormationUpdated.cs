using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formations.Events
{
    public class FormationUpdated : DomainEvent
    {
        public string Nom { get; }
        public int Places { get; }

        public FormationUpdated(Guid aggregateId, int sequence, string nom, int places) : base(aggregateId, sequence)
        {
            Nom = nom;
            Places = places;
        }
    }
}