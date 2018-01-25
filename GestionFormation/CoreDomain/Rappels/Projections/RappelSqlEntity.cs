using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionFormation.CoreDomain.Utilisateurs;

namespace GestionFormation.CoreDomain.Rappels.Projections
{
    [Table("Rappel")]
    public class RappelSqlEntity
    {
        [Key]
        public Guid Id { get; set; }        
        public string Label { get; set; }
        public UtilisateurRole AffectedRole { get; set; }
        public string AggregateType { get; set; }
        public Guid AggregateId { get; set; }
        public RappelType RappelType { get; set; }
        public Guid? SocieteId { get; set; }
    }
}