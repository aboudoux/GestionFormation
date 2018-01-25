using System;

namespace GestionFormation.EventStore
{
    public class EmptyEventStamping : IEventStamping
    {
        public DateTime GetDateTime()
        {
            throw new NotImplementedException();
        }

        public string GetCurrentUser()
        {
            throw new NotImplementedException();
        }
    }
}