using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Events
{
    public class SessionDeleted : DomainEvent
    {
        public SessionDeleted(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }
    }
}