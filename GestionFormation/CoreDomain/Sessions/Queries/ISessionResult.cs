using System;

namespace GestionFormation.CoreDomain.Sessions.Queries
{
    public interface ISessionResult
    {
        Guid SessionId { get; }
        Guid TrainingId { get; }
        DateTime SessionStart { get; }
        int Duration { get; }
        int Seats { get; }
        int ReservedSeats { get; }
        Guid? LocationId { get; }
        Guid? TrainerId { get; }
        bool Canceled { get; }
        string CancelReason { get; }
    }
}