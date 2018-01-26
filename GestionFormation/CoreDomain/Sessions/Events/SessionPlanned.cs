using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Events
{
    public class SessionPlanned : DomainEvent
    {
        public Guid FormationId { get; }
        public DateTime DateDebut { get; }
        public int DuréeEnJour { get; }
        public int NbrPlaces { get; }
        public Guid? LieuId { get; }
        public Guid? FormateurId { get; }

        public SessionPlanned(Guid aggregateId, int sequence, Guid formationId, DateTime dateDebut, int duréeEnJour, int nbrPlaces, Guid? lieuId, Guid? formateurId) : base(aggregateId, sequence)
        {
            FormationId = formationId;
            DateDebut = dateDebut;
            DuréeEnJour = duréeEnJour;
            NbrPlaces = nbrPlaces;
            LieuId = lieuId;
            FormateurId = formateurId;
        }

        protected override string Description => "Session planifiée";
    }
}