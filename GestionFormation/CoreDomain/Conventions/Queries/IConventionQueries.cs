using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Conventions.Queries
{
    public interface IConventionQueries
    {
        IEnumerable<IConventionResult> GetAll(Guid sessionId);

        IPrintableConventionResult GetPrintableConvention(Guid conventionId);


        long GetNextConventionNumber();
    }
}
