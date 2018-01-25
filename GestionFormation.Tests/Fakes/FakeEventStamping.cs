using System;
using GestionFormation.EventStore;

namespace GestionFormation.Tests.Fakes
{
    public class FakeEventStamping : IEventStamping
    {
        public DateTime GetDateTime()
        {
            return DateTime.Now;            
        }

        public string GetCurrentUser()
        {
            return "";
        }
    }
}