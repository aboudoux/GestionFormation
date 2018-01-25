using System;
using GestionFormation.CoreDomain.Places;
using GestionFormation.CoreDomain.Sessions.Events;
using GestionFormation.CoreDomain.Sessions.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions
{
    public class Session : AggregateRootUpdatableAndDeletable<SessionUpdated, SessionDeleted>
    {
        private bool _isCanceled;
        private int _placesDisponibles;
        private int _placesReservées;

        public Guid? FormateurId { get; private set; }
        public Guid? LieuId { get; private set; }
        public DateTime DateDebut { get; private set; }
        public int Durée { get; private set; }

        public Session(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            player.Add<SessionDeleted>(e => _isDeleted = true)                
                .Add<SessionUpdated>(e =>
                {
                    _lastUpdate = e;
                    FormateurId = e.FormateurId;
                    LieuId = e.LieuId;
                    DateDebut = e.DateDebut;
                    Durée = e.DuréeEnJour;
                    _placesDisponibles = e.NbrPlaces;
                })
                .Add<SessionCanceled>(e => _isCanceled = true)
                .Add<SessionPlanned>(e =>
                {
                    _placesDisponibles = e.NbrPlaces;
                    FormateurId = e.FormateurId;
                    LieuId = e.LieuId;
                    DateDebut = e.DateDebut;
                    Durée = e.DuréeEnJour;
                })
                .Add<SessionPlaceReserved>(e => _placesReservées++)
                .Add<SessionPlaceReleased>(e => _placesReservées--);
        }

        public static Session Plan(Guid formationId, DateTime dateDebut, int durée, int nbrPlaces, Guid? lieuId, Guid? formateurId)
        {
            if( formationId == Guid.Empty) throw new ArgumentNullException(nameof(formationId));

            var session = new Session(History.Empty);
            session.AggregateId = Guid.NewGuid();

            if(PeriodHaveWeekendDay(dateDebut, durée))
                throw new SessionWeekEndException();
                       
            var ev = new SessionPlanned(session.AggregateId, 1, formationId, dateDebut, durée, nbrPlaces, lieuId, formateurId);
            session.Apply(ev);
            session.UncommitedEvents.Add(ev);
            return session;
        }

        public void Update(Guid formationId, DateTime dateDebut, int durée, int nbrPlaces, Guid? lieuId, Guid? formateurId)
        {
            if (formationId == Guid.Empty) throw new ArgumentNullException(nameof(formationId));

            if (PeriodHaveWeekendDay(dateDebut, durée))
                throw new SessionWeekEndException();
            if (_placesReservées > nbrPlaces )
                throw new TooManyPlacesAlreadyReservedException(_placesReservées, nbrPlaces);

            Update(new SessionUpdated(AggregateId, GetNextSequence(), dateDebut, durée, nbrPlaces, lieuId, formateurId, formationId));            
        }

        public void Delete()
        {            
            Delete(new SessionDeleted(AggregateId, GetNextSequence()));
        }

        public void Cancel(string raison)
        {
            if(_isCanceled ||_isDeleted)
                return;

            if( string.IsNullOrWhiteSpace(raison))
                throw new ArgumentNullException(nameof(raison));

            RaiseEvent(new SessionCanceled(AggregateId, GetNextSequence(), raison));
        }

        public Place ReserverPlace(Guid stagiaireId, Guid societeId)
        {
            if( stagiaireId == Guid.Empty) throw new ArgumentNullException(nameof(stagiaireId));
            if(societeId == Guid.Empty) throw new ArgumentNullException(nameof(societeId));

            if(_placesDisponibles - _placesReservées <= 0)
                throw new NoMorePlacesAvailableException();

            RaiseEvent(new SessionPlaceReserved(AggregateId, GetNextSequence()));
            
            return Place.Create(AggregateId, stagiaireId, societeId);
        }

        public void ReleasePlace()
        {
            if(_placesReservées > 0)
                RaiseEvent(new SessionPlaceReleased(AggregateId, GetNextSequence()));
        }

        private static bool PeriodHaveWeekendDay(DateTime debut, int durée)
        {
            for (var i = 0; i < durée; i++)
            {
                var day = debut.AddDays(i).DayOfWeek;
                if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                    return true;
            }
            return false;
        }
    }
}