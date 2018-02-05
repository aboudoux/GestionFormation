using System;

namespace GestionFormation.CoreDomain.BookingNotifications.Events
{
    public class AgreementToSignSent : BookingNotificationEvent
    {
        public Guid AgreementId { get; }

        public AgreementToSignSent(Guid aggregateId, int sequence, Guid sessionId, Guid companyId, Guid agreementId) : base(aggregateId, sequence, sessionId, companyId)
        {
            AgreementId = agreementId;
        }

        protected override string Description => "Convention à retourner signée envoyé";
    }
}