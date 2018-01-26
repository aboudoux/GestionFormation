using System;

namespace GestionFormation.EventStore.Queries
{
    public interface IEventResult
    {
        string EventName { get; }
        string User { get; }
        DateTime TimeStamp { get; }
        string Data { get; }
    }
}