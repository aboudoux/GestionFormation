using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Sessions.Queries
{
    public interface ISessionQueries
    {
        IEnumerable<ISessionResult> GetAll();
        IEnumerable<ISessionResult> GetAll(Guid formationId);

        IEnumerable<ICompleteSessionResult> GetAllCompleteSession();
        IEnumerable<string> GetAllLieux();
        ICompleteSessionResult GetSession(Guid sessionId);
    }
}
