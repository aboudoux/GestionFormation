using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Sessions.Queries
{
    public interface ISessionQueries
    {
        IReadOnlyList<ISessionResult> GetAll();
        IReadOnlyList<ISessionResult> GetAll(Guid formationId);

        IReadOnlyList<ICompleteSessionResult> GetAllCompleteSession();
        IReadOnlyList<string> GetAllLieux();
        ICompleteSessionResult GetSession(Guid sessionId);
    }
}
