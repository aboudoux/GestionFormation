using System;
using GestionFormation.CoreDomain.Sessions.Queries;
using GestionFormation.Infrastructure.Sessions.Projections;

namespace GestionFormation.Infrastructure.Sessions.Queries
{
    public class SessionResult : ISessionResult
    {
        public SessionResult(SessionSqlEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            SessionId = entity.SessionId;
            TrainingId = entity.TrainingId;
            SessionStart = entity.SessionStart;
            Duration = entity.Duration;
            Seats = entity.Seats;
            ReservedSeats = entity.ReservedSeats;
            LocationId = entity.LocationId;
            TrainerId = entity.TrainerId;
            Canceled = entity.Canceled;
            CancelReason = entity.CancelReason;
        }

        public Guid SessionId { get; }
        public Guid TrainingId { get; }
        public DateTime SessionStart { get; }
        public int Duration { get; }
        public int Seats { get; set; }
        public int ReservedSeats { get; }
        public Guid? LocationId { get; }
        public Guid? TrainerId { get; }
        public bool Canceled { get; }
        public string CancelReason { get; }
    }
}