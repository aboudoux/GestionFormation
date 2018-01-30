using System;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Sessions.Events;
using GestionFormation.CoreDomain.Sessions.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions
{
    public class Session : AggregateRootUpdatableAndDeletable<SessionUpdated, SessionDeleted>
    {
        private bool _isCanceled;
        private int _availableSeats;
        private int _bookedSeats;

        public Guid? TrainerId { get; private set; }
        public Guid? LocationId { get; private set; }
        public DateTime SessionStart { get; private set; }
        public int Duration { get; private set; }

        public Session(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            player.Add<SessionDeleted>(e => _isDeleted = true)                
                .Add<SessionUpdated>(e =>
                {
                    _lastUpdate = e;
                    TrainerId = e.TrainerId;
                    LocationId = e.LocationId;
                    SessionStart = e.SessionStart;
                    Duration = e.Duration;
                    _availableSeats = e.Seats;
                })
                .Add<SessionCanceled>(e => _isCanceled = true)
                .Add<SessionPlanned>(e =>
                {
                    _availableSeats = e.Seats;
                    TrainerId = e.TrainerId;
                    LocationId = e.LocationId;
                    SessionStart = e.SessionStart;
                    Duration = e.Duration;
                })
                .Add<SessionSeatBooked>(e => _bookedSeats++)
                .Add<SessionSeatReleased>(e => _bookedSeats--);
        }

        public static Session Plan(Guid trainingId, DateTime sessionStart, int duration, int seats, Guid? locationId, Guid? trainerId)
        {
            if( trainingId == Guid.Empty) throw new ArgumentNullException(nameof(trainingId));

            var session = new Session(History.Empty);
            session.AggregateId = Guid.NewGuid();

            if(PeriodHaveWeekendDay(sessionStart, duration))
                throw new SessionWeekEndException();
                       
            var ev = new SessionPlanned(session.AggregateId, 1, trainingId, sessionStart, duration, seats, locationId, trainerId);
            session.Apply(ev);
            session.UncommitedEvents.Add(ev);
            return session;
        }

        public void Update(Guid trainingId, DateTime sessionStart, int duration, int seats, Guid? locationId, Guid? trainerId)
        {
            if (trainingId == Guid.Empty) throw new ArgumentNullException(nameof(trainingId));

            if (PeriodHaveWeekendDay(sessionStart, duration))
                throw new SessionWeekEndException();
            if (_bookedSeats > seats )
                throw new TooManySeatsAlreadyReservedException(_bookedSeats, seats);

            Update(new SessionUpdated(AggregateId, GetNextSequence(), sessionStart, duration, seats, locationId, trainerId, trainingId));            
        }

        public void Delete()
        {            
            Delete(new SessionDeleted(AggregateId, GetNextSequence()));
        }

        public void Cancel(string reason)
        {
            if(_isCanceled ||_isDeleted)
                return;

            if( string.IsNullOrWhiteSpace(reason))
                throw new ArgumentNullException(nameof(reason));

            RaiseEvent(new SessionCanceled(AggregateId, GetNextSequence(), reason));
        }

        public Seat BookSeat(Guid traineeId, Guid companyId)
        {
            if( traineeId == Guid.Empty) throw new ArgumentNullException(nameof(traineeId));
            if(companyId == Guid.Empty) throw new ArgumentNullException(nameof(companyId));

            if(_availableSeats - _bookedSeats <= 0)
                throw new NoMoreSeatAvailableException();

            RaiseEvent(new SessionSeatBooked(AggregateId, GetNextSequence()));
            
            return Seat.Create(AggregateId, traineeId, companyId);
        }

        public void ReleasePlace()
        {
            if(_bookedSeats > 0)
                RaiseEvent(new SessionSeatReleased(AggregateId, GetNextSequence()));
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