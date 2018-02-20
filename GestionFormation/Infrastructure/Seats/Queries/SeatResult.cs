using System;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Seats.Queries;
using GestionFormation.Infrastructure.Agreements.Projections;
using GestionFormation.Infrastructure.Seats.Projections;

namespace GestionFormation.Infrastructure.Seats.Queries
{
    public class SeatResult : ISeatResult
    {
        public SeatResult(SeatSqlentity a, AgreementSqlEntity agreement, string studentLastname, string studentFirstname, string companieName)
        {
            StudentLastname = studentLastname;
            StudentFirstname = studentFirstname;
            CompanyName = companieName;
            SeatId = a.SeatId;
            StudentId = a.StudentId;
            CompanyId = a.CompanyId;
            Status = a.Status;
            Reason = a.Reason;
            AgreementId = a.AssociatedAgreementId;
            AgreementRevoked = a.AgreementRevoked;
            SessionId = a.SessionId;            

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
        public Guid SessionId { get; }
        public bool AgreementRevoked { get; }
        public string StudentLastname { get; }
        public string StudentFirstname { get; }
        public string CompanyName { get; }
    }
}