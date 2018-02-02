using System;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Agreements.Projections;
using GestionFormation.CoreDomain.Seats.Projections;

namespace GestionFormation.CoreDomain.Seats.Queries
{
    public class SeatResult : ISeatResult
    {
        public SeatResult(SeatSqlentity a, AgreementSqlEntity agreement)
        {
            SeatId = a.SeatId;
            StudentId = a.StudentId;
            CompanyId = a.CompanyId;
            Status = a.Status;
            Reason = a.Reason;
            AgreementId = a.AssociatedAgreementId;

            if (agreement != null)
            {
                Agreementnumber = agreement.AgreementNumber;
                AgreementSigned = agreement.DocumentId.HasValue;
                AgreementType = agreement.AgreementTypeAgreement;
            }
        }

        public Guid SeatId { get; }
        public Guid StudentId { get; }
        public Guid CompanyId { get; }
        public SeatStatus Status { get; }
        public string Reason { get; }
        public Guid? AgreementId { get; }
        public string Agreementnumber { get; }
        public bool AgreementSigned { get; }
        public AgreementType AgreementType { get; }
    }
}