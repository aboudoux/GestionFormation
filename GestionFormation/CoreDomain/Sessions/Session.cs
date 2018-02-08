using System;
using System.Collections.Generic;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Sessions.Events;
using GestionFormation.CoreDomain.Sessions.Exceptions;
using GestionFormation.CoreDomain.Students.Events;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions
{
    public class Session : AggregateRootUpdatableAndDeletable<SessionUpdated, SessionDeleted>
    {
        private bool _isCanceled;
        private int _availableSeats;
        private int _bookedSeats;
        private HashSet<Guid> _reportedMissingStudents = new HashSet<Guid>();

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
                .Add<SessionSeatReleased>(e => _bookedSeats--)
                .Add<MissingStudentReported>(e=>_reportedMissingStudents.Add(e.StudentId));
        }

        public static Session Plan(Guid trainingId, DateTime sessionStart, int duration, int seats, Guid? locationId, Guid? trainerId)
        {
            trainingId.EnsureNotEmpty(nameof(trainingId));

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
            trainingId.EnsureNotEmpty(nameof(trainingId));

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

        public Seat BookSeat(Guid studentId, Guid companyId)
        {
            studentId.EnsureNotEmpty(nameof(studentId));
            companyId.EnsureNotEmpty(nameof(companyId));

            if(_availableSeats - _bookedSeats <= 0)
                throw new NoMoreSeatAvailableException();

            RaiseEvent(new SessionSeatBooked(AggregateId, GetNextSequence()));
            
            return Seat.Create(AggregateId, studentId, companyId);
        }

        public void ReleaseSeat()
        {
            if(_bookedSeats > 0)
                RaiseEvent(new SessionSeatReleased(AggregateId, GetNextSequence()));
        }

        public void ReportMissingStudent(Guid studentId)
        {
            if(_reportedMissingStudents.Contains(studentId)) return;
            RaiseEvent(new MissingStudentReported(AggregateId, GetNextSequence(), studentId));
        }

        private static bool PeriodHaveWeekendDay(DateTime start, int duration)
        {
            for (var i = 0; i < duration; i++)
            {
                var day = start.AddDays(i).DayOfWeek;
                if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                    return true;
            }
            return false;
        }
    }
}