using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Agreements.Queries
{
    public interface IAgreementResult
    {
        Guid AgreementId { get; }
        string Company { get; }
        string Contact { get; }
        List<Guid> Seats { get; }
        string AgreementNumber { get; }
    }
}