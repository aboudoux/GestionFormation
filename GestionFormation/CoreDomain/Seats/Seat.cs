using System;
using GestionFormation.CoreDomain.Agreements.Events;
using GestionFormation.CoreDomain.Seats.Events;
using GestionFormation.CoreDomain.Seats.Exceptions;
using GestionFormation.CoreDomain.Sessions.Events;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Seats
{
    public class Seat : AggregateRoot
    {
        private SeatStatus _currentSeatStatus = SeatStatus.ToValidate;
        public Guid? AssociatedAgreementId;

        public Guid SessionId { get; private set; }
        public Guid CompanyId { get; private set; }
        public Guid? StudentId { get; private set; }

        private bool _missingStudent;

        public Seat(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));
            player
                .Add<SeatCanceled>(e => _currentSeatStatus = SeatStatus.Canceled)
                .Add<SeatValided>(e => _currentSeatStatus = SeatStatus.Valid)
                .Add<SeatRefused>(e => _currentSeatStatus = SeatStatus.Refused)
                .Add<SeatCreated>(e =>
                {
                    SessionId = e.SessionId;
                    CompanyId = e.CompanyId;
                    StudentId = e.StudentId;
                })
                .Add<AgreementAssociated>(e => AssociatedAgreementId = e.AgreementId)
                .Add<AgreementDisassociated>(e => AssociatedAgreementId = null)
                .Add<MissingStudentReported>(e => _missingStudent = true)
                .Add<SeatStudentUpdated>(e => StudentId = e.NewStudentId);
        }

        public static Seat Create(Guid sessionId, Guid? studentId, Guid companyId)
        {
            sessionId.EnsureNotEmpty(nameof(sessionId));
            companyId.EnsureNotEmpty(nameof(companyId));

            var seat = new Seat(History.Empty);
            seat.AggregateId = Guid.NewGuid();

            var @event = new SeatCreated(seat.AggregateId, 1, sessionId, studentId, companyId);
            seat.Apply(@event);
            seat.UncommitedEvents.Add(@event);
            
            return seat;
        }
      
        public void Cancel(string reason)
        {
            if (_currentSeatStatus == SeatStatus.Canceled) return;

            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentNullException(nameof(reason));

            RaiseEvent(new SeatCanceled(AggregateId, GetNextSequence(), reason));
        }       

        public void Validate()
        {
            if (!StudentId.HasValue)
                throw new UndefinedStudentExceptionValidationException();            
            if (_currentSeatStatus == SeatStatus.Valid) return;
            RaiseEvent(new SeatValided(AggregateId, GetNextSequence()));
        }

        public void Refuse(string reason)
        {
            if (_currentSeatStatus == SeatStatus.Refused) return;

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentNullException(nameof(reason));

            RaiseEvent(new SeatRefused(AggregateId, GetNextSequence(), reason));
        }

        public void AssociateAgreement(Guid agreementId)
        {
            agreementId.EnsureNotEmpty(nameof(agreementId));

            if (_currentSeatStatus != SeatStatus.Valid)
                throw new AssignAgreementException();
            RaiseEvent(new AgreementAssociated(AggregateId, GetNextSequence(), agreementId));
        }

        public void DisassociateAgreement()
        {
            if(AssociatedAgreementId.HasValue)
                RaiseEvent(new AgreementDisassociated(AggregateId, GetNextSequence()));
        }

        public void ReportMissingStudent()
        {
            if(!_missingStudent)
                RaiseEvent(new MissingStudentReported(AggregateId, GetNextSequence()));
        }

        public void SendCertificatOfAttendance(Guid documentId)
        {
            RaiseEvent(new CertificatOfAttendanceSent(AggregateId, GetNextSequence(), documentId));
        }

        public void UpdateStudent(Guid? newStudentId)
        {
            if(newStudentId != StudentId)
                RaiseEvent(new SeatStudentUpdated(AggregateId, GetNextSequence(), newStudentId));
        }
    }
}
