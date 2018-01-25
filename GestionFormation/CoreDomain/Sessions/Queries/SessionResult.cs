using System;
using GestionFormation.CoreDomain.Sessions.Projections;

namespace GestionFormation.CoreDomain.Sessions.Queries
{
    public class SessionResult : ISessionResult
    {
        public SessionResult(SessionSqlEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            SessionId = entity.SessionId;
            FormationId = entity.FormationId;
            DateDebut = entity.DateDebut;
            Durée = entity.DuréeEnJour;
            Places = entity.Places;
            PlacesReservées = entity.PlacesReservées;
            LieuId = entity.LieuId;
            FormateurId = entity.FormateurId;
            Annulé = entity.Annulé;
            RaisonAnnulation = entity.RaisonAnnulation;
        }

        public Guid SessionId { get; set; }
        public Guid FormationId { get; set; }
        public DateTime DateDebut { get; set; }
        public int Durée { get; set; }
        public int Places { get; set; }
        public int PlacesReservées { get; set; }
        public Guid? LieuId { get; set; }
        public Guid? FormateurId { get; set; }
        public bool Annulé { get; set; }
        public string RaisonAnnulation { get; set; }
    }
}