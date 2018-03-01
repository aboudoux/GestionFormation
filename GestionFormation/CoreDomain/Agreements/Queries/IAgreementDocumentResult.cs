using System;

namespace GestionFormation.CoreDomain.Agreements.Queries
{
    public interface IAgreementDocumentResult
    {
        Guid? SignedDocumentId { get; }
        AgreementType Type { get; }
    }
}