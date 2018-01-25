using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Stagiaires.Projections
{
    [Table("Stagiaire")]
    public class StagiaireSqlEntity
    {
        [Key]
        public Guid StagiaireId { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
    }
}