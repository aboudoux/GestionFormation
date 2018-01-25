using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Events
{
    public class SessionPlaceReserved : DomainEvent
    {
        public SessionPlaceReserved(Guid aggregateId, int sequence) : base(aggregateId, sequence)
        {
        }
    }
}