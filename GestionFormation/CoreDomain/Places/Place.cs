using System;
using GestionFormation.CoreDomain.Conventions.Events;
using GestionFormation.CoreDomain.Places.Events;
using GestionFormation.CoreDomain.Places.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Places
{
    public class Place : AggregateRoot
    {
        private PlaceStatus _currentPlaceStatus = PlaceStatus.AValider;
        public Guid? AssociatedConventionId;

        public Guid SessionId { get; private set; }
        public Guid SocieteId { get; private set; }


        public Place(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));            
            player
                .Add<PlaceCanceled>(e => _currentPlaceStatus = PlaceStatus.Annulé)
                .Add<PlaceValided>(e => _currentPlaceStatus = PlaceStatus.Validé)
                .Add<PlaceRefused>(e => _currentPlaceStatus = PlaceStatus.Refusé)
                .Add<PlaceCreated>(e =>
                {
                    SessionId = e.SessionId;
                    SocieteId = e.SocieteId;
                })
                .Add<ConventionAssociated>(e => AssociatedConventionId = e.ConventionId)
                .Add<ConventionDisassociated>(e => AssociatedConventionId = null);
        }

        public static Place Create(Guid sessionId, Guid stagiaireId, Guid societeId)
        {
            if(sessionId == Guid.Empty) throw new ArgumentNullException(nameof(sessionId));
            if(stagiaireId == Guid.Empty) throw new ArgumentNullException(nameof(stagiaireId));
            if(societeId == Guid.Empty) throw new ArgumentNullException(nameof(societeId));

            var place = new Place(History.Empty);
            place.AggregateId = Guid.NewGuid();

            var @event = new PlaceCreated(place.AggregateId, 1, sessionId, stagiaireId, societeId);
            place.Apply(@event);
            place.UncommitedEvents.Add(@event);
            
            return place;
        }
      
        public void Cancel(string raison)
        {
            if (_currentPlaceStatus == PlaceStatus.Annulé) return;

            if (string.IsNullOrWhiteSpace(raison)) throw new ArgumentNullException(nameof(raison));

            RaiseEvent(new PlaceCanceled(AggregateId, GetNextSequence(), raison));
        }       

        public void Validate()
        {
            if (_currentPlaceStatus != PlaceStatus.AValider)
                throw new ValidatePlaceException();
            if (_currentPlaceStatus == PlaceStatus.Validé) return;
            RaiseEvent(new PlaceValided(AggregateId, GetNextSequence()));
        }

        public void Refuse(string raison)
        {
            if (_currentPlaceStatus == PlaceStatus.Refusé) return;

            if (string.IsNullOrWhiteSpace(raison))
                throw new ArgumentNullException(nameof(raison));

            RaiseEvent(new PlaceRefused(AggregateId, GetNextSequence(), raison));
        }

        public void AssociateConvention(Guid conventionId)
        {
            if(conventionId == Guid.Empty) throw new ArgumentNullException(nameof(conventionId));
            if (_currentPlaceStatus != PlaceStatus.Validé)
                throw new AssignConventionException();
            RaiseEvent(new ConventionAssociated(AggregateId, GetNextSequence(), conventionId));
        }

        public void DisassociateConvention()
        {
            if(AssociatedConventionId.HasValue)
                RaiseEvent(new ConventionDisassociated(AggregateId, GetNextSequence()));
        }
    }
}
