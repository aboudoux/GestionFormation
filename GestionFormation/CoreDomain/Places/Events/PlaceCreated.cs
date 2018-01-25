using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Places.Events
{
    public class PlaceCreated : DomainEvent
    {
        public Guid SessionId { get; }
        public Guid StagiaireId { get; }
        public Guid SocieteId { get; }

        public PlaceCreated(Guid aggregateId, int sequence, Guid sessionId, Guid stagiaireId, Guid societeId) : base(aggregateId, sequence)
        {
            SessionId = sessionId;
            StagiaireId = stagiaireId;
            SocieteId = societeId;
        }
    }
}