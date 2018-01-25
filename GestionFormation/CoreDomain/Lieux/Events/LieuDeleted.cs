using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Lieux.Events
{
    public class LieuDeleted : DomainEvent
    {
        public LieuDeleted(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }
    }
}