using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formateurs.Events
{
    public class FormateurUnassigned : DomainEvent
    {
        public DateTime DateDebut { get; }
        public int Durée { get; }

        public FormateurUnassigned(Guid aggregateId, int sequence, DateTime dateDebut, int durée) : base(aggregateId, sequence)
        {
            DateDebut = dateDebut;
            Durée = durée;
        }
    }
}