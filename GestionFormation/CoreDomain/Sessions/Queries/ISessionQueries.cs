using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Sessions.Queries
{
    public interface ISessionQueries
    {
        IEnumerable<ISessionResult> GetAll();
        IEnumerable<ISessionResult> GetAll(Guid TrainingId);

        IEnumerable<ICompleteSessionResult> GetAllCompleteSession();
        IEnumerable<string> GetAllLocation();
        ICompleteSessionResult GetSession(Guid sessionId);
    }
}
