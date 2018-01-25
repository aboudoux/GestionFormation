using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Events
{
    public class SessionCanceled : DomainEvent
    {
        public string Raison { get; }

        public SessionCanceled(Guid aggregateId, int sequence, string raison) : base(aggregateId, sequence)
        {
            Raison = raison;
        }
    }
}