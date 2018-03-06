using System;

namespace GestionFormation.CoreDomain.Agreements.Queries
{
    public interface IPrintableAgreementResult
    {
        string AgreementNumber { get; }
        AgreementType AgreementType { get; }
        string Training { get; }
        string Location { get; }
        DateTime StartDate { get; }
        int Duration { get; }
        
        decimal PricePerDayAndPerStudent { get; }
        decimal PackagePrice { get; }
    }
}