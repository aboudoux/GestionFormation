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
        private readonly HashSet<Guid> _bookedStudent = new HashSet<Guid>();

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
                .Add<SessionSeatBooked>(e =>
                {
                    _bookedSeats++;
                    if(e.StudentId.HasValue)
                        _bookedStudent.Add(e.StudentId.Value);
                })
                .Add<SessionSeatReleased>(e =>
                {
                    _bookedSeats--;
                    _bookedStudent.Remove(e.StudentId);
                });
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

        public Seat BookSeat(Guid? studentId, Guid companyId)
        {
            companyId.EnsureNotEmpty(nameof(companyId));

            if(_availableSeats - _bookedSeats <= 0)
                throw new NoMoreSeatAvailableException();

            RaiseEvent(new SessionSeatBooked(AggregateId, GetNextSequence(), studentId));
            
            return Seat.Create(AggregateId, studentId, companyId);
        }

        public void ReleaseSeat(Guid studentId)
        {
            if(_bookedSeats > 0)
                RaiseEvent(new SessionSeatReleased(AggregateId, GetNextSequence(), studentId));
        }
      
        public void SendSurvey(Guid documentId)
        {
            GuidAssert.AreNotEmpty(documentId);
            RaiseEvent(new SessionSurveySent(AggregateId, GetNextSequence(), documentId));
        }

        public void SendTimesheet(Guid documentId)
        {
            GuidAssert.AreNotEmpty(documentId);
            RaiseEvent(new SessionTimesheetSent(AggregateId, GetNextSequence(), documentId));
        }

        public void SendCertificateOfAttendance(Guid studentId, Guid documentId)
        {
            GuidAssert.AreNotEmpty(studentId, documentId);
            if(!_bookedStudent.Contains(studentId))
                throw new StudentNotInSessionException();
            RaiseEvent(new CertificateOfAttendanceSent(AggregateId, GetNextSequence(), studentId, documentId));
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