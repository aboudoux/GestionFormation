using System;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Agreements.Queries;

namespace GestionFormation.Infrastructure.Agreements.Queries
{
    public class AgreementDocumentResult : IAgreementDocumentResult
    {
        public AgreementDocumentResult(Guid? signedDocumentId, AgreementType type)
        {
            SignedDocumentId = signedDocumentId;
            Type = type;
        }
        public Guid? SignedDocumentId { get; }
        public AgreementType Type { get; }
    }
}