using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Lieux.Events
{
    public class LieuAssigned : DomainEvent
    {
        public DateTime DebutSession { get; }
        public int Durée { get; }

        public LieuAssigned(Guid aggregateId, int sequence, DateTime debutSession, int durée) : base(aggregateId, sequence)
        {
            DebutSession = debutSession;
            Durée = durée;
        }
    }
}