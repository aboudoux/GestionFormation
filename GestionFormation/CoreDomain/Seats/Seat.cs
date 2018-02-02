using System;
using GestionFormation.CoreDomain.Agreements.Events;
using GestionFormation.CoreDomain.Seats.Events;
using GestionFormation.CoreDomain.Seats.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Seats
{
    public class Seat : AggregateRoot
    {
        private SeatStatus _currentSeatStatus = SeatStatus.ToValidate;
        public Guid? AssociatedAgreementId;

        public Guid SessionId { get; private set; }
        public Guid CompanyId { get; private set; }


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
                })
                .Add<AgreementAssociated>(e => AssociatedAgreementId = e.AgreementId)
                .Add<AgreementDisassociated>(e => AssociatedAgreementId = null);
        }

        public static Seat Create(Guid sessionId, Guid studentId, Guid companyId)
        {
            if(sessionId == Guid.Empty) throw new ArgumentNullException(nameof(sessionId));
            if(studentId == Guid.Empty) throw new ArgumentNullException(nameof(studentId));
            if(companyId == Guid.Empty) throw new ArgumentNullException(nameof(companyId));

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
            if (_currentSeatStatus != SeatStatus.ToValidate)
                throw new ValidateSeatException();
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
            if(agreementId == Guid.Empty) throw new ArgumentNullException(nameof(agreementId));
            if (_currentSeatStatus != SeatStatus.Valid)
                throw new AssignAgreementException();
            RaiseEvent(new AgreementAssociated(AggregateId, GetNextSequence(), agreementId));
        }

        public void DisassociateAgreement()
        {
            if(AssociatedAgreementId.HasValue)
                RaiseEvent(new AgreementDisassociated(AggregateId, GetNextSequence()));
        }
    }
}
