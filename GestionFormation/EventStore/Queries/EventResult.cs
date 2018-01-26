using System;
using GestionFormation.Kernel;

namespace GestionFormation.EventStore.Queries
{
    public class EventResult : IEventResult
    {
        public EventResult(DbEvent dbEvent)
        {
            EventName = dbEvent.EventName;
            User = dbEvent.User;
            TimeStamp = dbEvent.TimeStamp;
            Data = dbEvent.Data;
        }

        public string EventName { get; }
        public string User { get; }
        public DateTime TimeStamp { get; }
        public string Data { get; }
    }
}