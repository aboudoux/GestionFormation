using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionFormation.CoreDomain.Agreements;

namespace GestionFormation.Infrastructure.Agreements.Projections
{
    [Table("Agreement")]
    public class AgreementSqlEntity
    {
        [Key]
        public Guid AgreementId { get; set; }
        public Guid ContactId { get; set; }
        public Guid? DocumentId { get; set; }    
        public string AgreementNumber { get; set; }
        public AgreementType AgreementTypeAgreement { get; set; }
    }
}