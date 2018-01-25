using System;

namespace GestionFormation.EventStore
{
    public interface IEventStamping
    {
        DateTime GetDateTime();
        string GetCurrentUser();
    }
}