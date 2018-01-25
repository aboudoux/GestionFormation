using System;
using System.Collections.Generic;
using GestionFormation.CoreDomain.Conventions.Queries;

namespace GestionFormation.Tests.Fakes
{
    public class FakeConventionQueries : IConventionQueries
    {
        public IEnumerable<IConventionResult> GetAll(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        public long GetNextConventionNumber()
        {
            return 6001;
        }
    }
}