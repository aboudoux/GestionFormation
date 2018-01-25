using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Events
{
    public class SessionUpdated : DomainEvent
    {
        public DateTime DateDebut { get; }
        public int DuréeEnJour { get; }
        public int NbrPlaces { get; }
        public Guid? LieuId { get; }
        public Guid? FormateurId { get; }
        public Guid FormationId { get; }

        public SessionUpdated(Guid aggregateId, int sequence, DateTime dateDebut, int duréeEnJour, int nbrPlaces, Guid? lieuId, Guid? formateurId, Guid formationId) : base(aggregateId, sequence)
        {
            DateDebut = dateDebut;
            DuréeEnJour = duréeEnJour;
            NbrPlaces = nbrPlaces;
            LieuId = lieuId;
            FormateurId = formateurId;
            FormationId = formationId;
        }
    }
}