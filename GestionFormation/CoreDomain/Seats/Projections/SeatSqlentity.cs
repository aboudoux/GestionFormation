using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Seats.Projections
{
    [Table("Seat")]
    public class SeatSqlentity
    {
        [Key]
        public Guid SeatId { get; set; }
        public Guid SessionId { get; set; }
        public Guid StudentId { get; set; }
        public Guid CompanyId { get; set; }
        public SeatStatus Status { get; set; }
        public string Reason { get; set; }
        public Guid? AssociatedAgreementId { get; set; }        
    }
}