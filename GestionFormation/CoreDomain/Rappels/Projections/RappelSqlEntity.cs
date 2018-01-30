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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid? PlaceId { get; set; }
        public Guid? SessionId { get; set; }        
        public Guid? SocieteId { get; set; }
        
        public string Label { get; set; }
        public UtilisateurRole AffectedRole { get; set; }
        
        public Guid? ConventionId { get; set; }

        public RappelType RappelType { get; set; }
        
    }
}