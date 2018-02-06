using System;

namespace GestionFormation.CoreDomain.BookingNotifications.Events
{
    public class AgreementToSignNotificationSent : BookingNotificationEvent
    {
        public Guid AgreementId { get; }

        public AgreementToSignNotificationSent(Guid aggregateId, int sequence, Guid sessionId, Guid companyId, Guid agreementId) : base(aggregateId, sequence, sessionId, companyId)
        {
            AgreementId = agreementId;
        }

        protected override string Description => "Convention à retourner signée envoyé";
    }
}