using System;
using GestionFormation.CoreDomain.Agreements;

namespace GestionFormation.CoreDomain.Seats.Queries
{
    public interface ISeatResult
    {
        Guid SeatId { get; }
        Guid TraineeId { get; }
        Guid CompanyId { get; }
        SeatStatus Status { get; }
        string Reason { get; }
        Guid? AgreementId { get; }
        string Agreementnumber { get; }
        bool AgreementSigned { get; }
        AgreementType AgreementType { get; }
    }
}