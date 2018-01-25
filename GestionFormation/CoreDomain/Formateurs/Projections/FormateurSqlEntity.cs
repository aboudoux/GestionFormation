using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Formateurs.Projections
{
    [Table("Formateur")]
    public class FormateurSqlEntity
    {
        [Key]
        public Guid FormateurId { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }

    }
}