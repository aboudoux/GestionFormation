﻿using System;
using System.Collections.Generic;

namespace GestionFormation.CoreDomain.Agreements.Queries
{
    public interface IAgreementQueries
    {
        IEnumerable<IAgreementResult> GetAll(Guid sessionId);

        IPrintableAgreementResult GetPrintableAgreement(Guid agreementId);

        Guid? GetSignedAgreementDocumentId(Guid agreementId);

        long GetNextAgreementNumber();
    }
}
