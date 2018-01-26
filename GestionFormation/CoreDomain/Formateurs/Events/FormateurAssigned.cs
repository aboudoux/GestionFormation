using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formateurs.Events
{
    public class FormateurAssigned : DomainEvent
    {
        public DateTime DebutSession { get; }
        public int Durée { get; }

        public FormateurAssigned(Guid aggregateId, int sequence, DateTime debutSession, int durée) : base(aggregateId, sequence)
        {
            DebutSession = debutSession;
            Durée = durée;
        }

        protected override string Description => "Formateur assigné";
    }
}