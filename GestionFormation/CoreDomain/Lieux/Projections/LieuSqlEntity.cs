using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Lieux.Projections
{
    [Table("Lieu")]
    public class LieuSqlEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Nom { get; set; }
        public string Addresse { get; set; }
        public bool Actif { get; set; }
        public int Places { get; set; }
    }
}