using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Events
{
    public class SessionPlanned : DomainEvent
    {
        public Guid TrainingId { get; }
        public DateTime SessionStart { get; }
        public int Duration { get; }
        public int Seats { get; }
        public Guid? LocationId { get; }
        public Guid? TrainerId { get; }

        public SessionPlanned(Guid aggregateId, int sequence, Guid trainingId, DateTime sessionStart, int duration, int seats, Guid? locationId, Guid? trainerId) : base(aggregateId, sequence)
        {
            TrainingId = trainingId;
            SessionStart = sessionStart;
            Duration = duration;
            Seats = seats;
            LocationId = locationId;
            TrainerId = trainerId;
        }

        protected override string Description => "Session planifiée";
    }
}