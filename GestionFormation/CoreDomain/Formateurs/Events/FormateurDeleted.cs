using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formateurs.Events
{
    public class FormateurDeleted : DomainEvent
    {
        public FormateurDeleted(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }
    }
}