using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Lieux.Events
{
    public class LieuUnassigned : DomainEvent
    {
        public DateTime DateDebut { get; }
        public int Durée { get; }

        public LieuUnassigned(Guid aggregateId, int sequence, DateTime dateDebut, int durée) : base(aggregateId, sequence)
        {
            DateDebut = dateDebut;
            Durée = durée;
        }
    }
}