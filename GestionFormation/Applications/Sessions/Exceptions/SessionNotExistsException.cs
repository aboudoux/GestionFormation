using System;

namespace GestionFormation.Applications.Sessions
{
    public class SessionNotExistsException : Exception
    {
        public SessionNotExistsException(Guid sessionId) : base($"Impossible de charger la session avec l'identifiant {sessionId}")
        {
            
        }
    }
}