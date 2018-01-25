using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Sessions.Projections
{
    [Table("Session")]
    public class SessionSqlEntity
    {
        [Key]
        public Guid SessionId { get; set; }
        public Guid FormationId { get; set; }        
        public DateTime DateDebut { get; set; }
        public int DuréeEnJour { get; set; }
        public int Places { get; set; }
        public int PlacesReservées { get; set; }
        public Guid? LieuId { get; set; }
        public Guid? FormateurId { get; set; }

        public bool Annulé { get; set; }
        public string RaisonAnnulation { get; set; }
    }
}