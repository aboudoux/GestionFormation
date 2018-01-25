using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formations.Events
{
    public class FormationDeleted : DomainEvent
    {
        public FormationDeleted(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }
    }
}