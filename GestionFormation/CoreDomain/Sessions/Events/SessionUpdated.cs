using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Events
{
    public class SessionUpdated : DomainEvent
    {
        public DateTime SessionStart { get; }
        public int Duration { get; }
        public int Seats { get; }
        public Guid? LocationId { get; }
        public Guid? TrainerId { get; }
        public Guid TrainingId { get; }

        public SessionUpdated(Guid aggregateId, int sequence, DateTime sessionStart, int duration, int seats, Guid? locationId, Guid? trainerId, Guid trainingId) : base(aggregateId, sequence)
        {
            SessionStart = sessionStart;
            Duration = duration;
            Seats = seats;
            LocationId = locationId;
            TrainerId = trainerId;
            TrainingId = trainingId;
        }

        protected override string Description => "Session modifiée";
    }
}