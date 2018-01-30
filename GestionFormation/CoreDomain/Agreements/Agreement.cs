using System;
using GestionFormation.CoreDomain.Agreements.Events;
using GestionFormation.CoreDomain.Agreements.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Agreements
{
    public class Agreement : AggregateRoot
    {
        private bool _isRevoked;
        public Agreement(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            player.Add<AgreementRevoked>(e => _isRevoked = true);
        }

        public static Agreement Create(Guid contactId, long numeroConvention, AgreementType agreementType)
        {
            if(contactId == Guid.Empty)
                throw new ArgumentNullException(nameof(contactId));

            var convention = new Agreement(History.Empty);
            convention.AggregateId = Guid.NewGuid();
            convention.UncommitedEvents.Add(new AgreementCreated(convention.AggregateId, 1, contactId, numeroConvention, agreementType));
            return convention;
        }

        public void Revoke()
        {
            if(!_isRevoked)
                RaiseEvent(new AgreementRevoked(AggregateId, GetNextSequence()));
        }

        public void Sign(Guid documentId)
        {
            if(documentId == Guid.Empty)
                throw new ArgumentNullException(nameof(documentId));

            if (_isRevoked)
                throw new CannotSignRevokedAgreement();

            RaiseEvent(new AgreementSigned(AggregateId, GetNextSequence(), documentId));
        }
    }
}
