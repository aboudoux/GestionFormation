using System;
using GestionFormation.CoreDomain.Agreements;

namespace GestionFormation.CoreDomain.Seats.Queries
{
    public interface ISeatResult
    {
        Guid SeatId { get; }
        Guid? StudentId { get; }
        Guid CompanyId { get; }
        SeatStatus Status { get; }
        string Reason { get; }
        Guid? AgreementId { get; }
        string Agreementnumber { get; }
        bool AgreementSigned { get; }
        AgreementType AgreementType { get; }
        Guid SessionId { get; }
        bool AgreementRevoked { get; }

        string StudentLastname { get; }
        string StudentFirstname { get; }

        string CompanyName { get; }
    }
}