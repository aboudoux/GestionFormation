using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Lieux.Events
{
    public class LieuReassigned : DomainEvent
    {
        public DateTime OldDateDebut { get; }
        public int OldDurée { get; }
        public DateTime NewDateDebut { get; }
        public int NewDurée { get; }

        public LieuReassigned(Guid aggregateId, int sequence, DateTime oldDateDebut, int oldDurée, DateTime newDateDebut, int newDurée) : base(aggregateId, sequence)
        {
            OldDateDebut = oldDateDebut;
            OldDurée = oldDurée;
            NewDateDebut = newDateDebut;
            NewDurée = newDurée;
        }

        protected override string Description => "Lieu réassigné";
    }
}