using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Societes.Projections
{
    [Table("Societe")]
    public class SocieteSqlEntity
    {
        [Key]
        public Guid SocieteId { get; set; }
        public string Nom { get; set; }
        public string Addresse { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
    }
}