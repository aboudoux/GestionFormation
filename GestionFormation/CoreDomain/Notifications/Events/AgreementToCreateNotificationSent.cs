using System;

namespace GestionFormation.CoreDomain.Notifications.Events
{
    public class AgreementToCreateNotificationSent : NotificationEvent
    {
        public AgreementToCreateNotificationSent(Guid aggregateId, int sequence, Guid sessionId, Guid companyId, Guid notificationId) : base(aggregateId, sequence, sessionId, companyId, notificationId)
        {
        }

        protected override string Description => "Convention à créer envoyé";
    }
}