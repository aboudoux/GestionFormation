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

        public Guid SessionId { get; }
        public Guid FormationId { get; }
        public DateTime DateDebut { get; }
        public int Durée { get; }
        public int Places { get; set; }
        public int PlacesReservées { get; }
        public Guid? LieuId { get; }
        public Guid? FormateurId { get; }
        public bool Annulé { get; }
        public string RaisonAnnulation { get; }
    }
}