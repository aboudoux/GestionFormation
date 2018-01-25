using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Conventions.Projections
{
    [Table("Convention")]
    public class ConventionSqlEntity
    {
        [Key]
        public Guid ConventionId { get; set; }
        public Guid ContactId { get; set; }
        public Guid? DocumentId { get; set; }    
        public string ConventionNumber { get; set; }
    }
}