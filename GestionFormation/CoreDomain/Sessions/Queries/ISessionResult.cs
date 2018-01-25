using System;

namespace GestionFormation.CoreDomain.Sessions.Queries
{
    public interface ISessionResult
    {
        Guid SessionId { get; }
        Guid FormationId { get; }
        DateTime DateDebut { get; }
        int Durée { get; }
        int Places { get; }
        int PlacesReservées { get; }
        Guid? LieuId { get; }
        Guid? FormateurId { get; }
        bool Annulé { get; }
        string RaisonAnnulation { get; }
    }
}