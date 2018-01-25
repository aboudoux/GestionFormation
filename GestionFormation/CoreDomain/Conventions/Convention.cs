using System;
using GestionFormation.CoreDomain.Conventions.Events;
using GestionFormation.CoreDomain.Conventions.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Conventions
{
    public class Convention : AggregateRoot
    {
        private bool _isRevoked;
        public Convention(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            player.Add<ConventionRevoked>(e => _isRevoked = true);
        }

        public static Convention Create(Guid contactId, long numeroConvention)
        {
            if(contactId == Guid.Empty)
                throw new ArgumentNullException(nameof(contactId));

            var convention = new Convention(History.Empty);
            convention.AggregateId = Guid.NewGuid();
            convention.UncommitedEvents.Add(new ConventionCreated(convention.AggregateId, 1, contactId, numeroConvention));
            return convention;
        }

        public void Revoke()
        {
            if(!_isRevoked)
                RaiseEvent(new ConventionRevoked(AggregateId, GetNextSequence()));
        }

        public void Sign(Guid documentId)
        {
            if(documentId == Guid.Empty)
                throw new ArgumentNullException(nameof(documentId));

            if (_isRevoked)
                throw new CannotSignRevokedConvention();

            RaiseEvent(new ConventionSigned(AggregateId, GetNextSequence(), documentId));
        }
    }
}
