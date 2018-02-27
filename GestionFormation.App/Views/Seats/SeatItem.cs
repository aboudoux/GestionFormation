using System;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Seats.Queries;

namespace GestionFormation.App.Views.Seats
{
    public class SeatItem
    {
        public SeatItem(ISeatResult result, string studentName, string companyName)
        {
            StudentName = studentName;
            CompanyName = companyName;
            SeatId = result.SeatId;
            StudentId = result.StudentId;
            CompanyId = result.CompanyId;
            Statut = result.Status;
            Reason = result.Reason;
            AgreementId = result.AgreementId;
            Agreement = result.Agreementnumber;
            AgreementType = result.AgreementType;

            AgreementState = new AgreementState(result);
            SeatState = new SeatState(result.Status);            
        }

        public Guid SeatId { get; }

        public Guid StudentId { get; }
        public Guid CompanyId { get;  }

        public string StudentName { get; }
        public string CompanyName { get; }

        public SeatStatus Statut { get; set; }
        public string Reason { get; }

        public SeatState SeatState { get; }
        
        public string Agreement { get; }
        public AgreementState AgreementState { get; }
        public Guid? AgreementId { get; }
        public AgreementType AgreementType { get; }               
    }
}