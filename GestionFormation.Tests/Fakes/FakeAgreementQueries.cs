using System;
using System.Collections.Generic;
using GestionFormation.CoreDomain.Agreements.Queries;

namespace GestionFormation.Tests.Fakes
{
    public class FakeAgreementQueries : IAgreementQueries
    {
        public IEnumerable<IAgreementResult> GetAll(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        public IPrintableAgreementResult GetPrintableAgreement(Guid agreementId)
        {
            throw new NotImplementedException();
        }

        public Guid? GetAgreementDocument(Guid agreementId)
        {
            throw new NotImplementedException();
        }

        public long GetNextAgreementNumber()
        {
            return 6001;
        }
    }
}