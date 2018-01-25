using System;

namespace GestionFormation.EventStore
{
    public class EventStamping : IEventStamping
    {
        private readonly ILoggedUser _loggedUser;

        public EventStamping(ILoggedUser loggedUser)
        {
            _loggedUser = loggedUser ?? throw new ArgumentNullException(nameof(loggedUser));
        }

        public DateTime GetDateTime()
        {
            return DateTime.Now;            
        }

        public string GetCurrentUser()
        {
            return $"{_loggedUser.Login} ({_loggedUser.Nom} {_loggedUser.Prenom})";
        }
    }
}