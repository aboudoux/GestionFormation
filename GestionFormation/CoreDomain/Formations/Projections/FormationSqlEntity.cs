using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Formations.Projections
{
    [Table("Formation")]
    public class FormationSqlEntity
    {
        [Key]
        public Guid FormationId { get; set; }
        public string Nom { get; set; }
        public int Places { get; set; }
    }
}