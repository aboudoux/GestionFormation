using System;

namespace GestionFormation.CoreDomain.Notifications.Events
{
    public class AgreementToSignNotificationSent : NotificationEvent
    {
        public Guid AgreementId { get; }

        public AgreementToSignNotificationSent(Guid aggregateId, int sequence, Guid sessionId, Guid companyId, Guid agreementId, Guid notificationId) : base(aggregateId, sequence, sessionId, companyId, notificationId)
        {
            AgreementId = agreementId;
        }

        protected override string Description => "Convention à retourner signée envoyé";
    }
}