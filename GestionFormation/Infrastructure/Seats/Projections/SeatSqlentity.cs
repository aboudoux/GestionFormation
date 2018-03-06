using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionFormation.CoreDomain.Seats;

namespace GestionFormation.Infrastructure.Seats.Projections
{
    [Table("Seat")]
    public class SeatSqlentity
    {
        [Key]
        public Guid SeatId { get; set; }
        public Guid SessionId { get; set; }
        public Guid? StudentId { get; set; }
        public Guid CompanyId { get; set; }
        public SeatStatus Status { get; set; }
        public string Reason { get; set; }
        public Guid? AssociatedAgreementId { get; set; }        
        public bool AgreementRevoked { get; set; }
        public Guid? CertificateOfAttendanceId { get; set; }
        public bool StudentMissing { get; set; }
    }
}