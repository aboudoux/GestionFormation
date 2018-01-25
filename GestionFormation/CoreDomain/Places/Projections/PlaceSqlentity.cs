using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Places.Projections
{
    [Table("Place")]
    public class PlaceSqlentity
    {
        [Key]
        public Guid PlaceId { get; set; }
        public Guid SessionId { get; set; }
        public Guid StagiaireId { get; set; }
        public Guid SocieteId { get; set; }
        public PlaceStatus Status { get; set; }
        public string Raison { get; set; }
        public Guid? AssociatedConventionId { get; set; }        
    }
}