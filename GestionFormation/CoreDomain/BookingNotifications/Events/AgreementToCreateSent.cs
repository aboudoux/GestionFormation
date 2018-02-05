using System;

namespace GestionFormation.CoreDomain.BookingNotifications.Events
{
    public class AgreementToCreateSent : BookingNotificationEvent
    {
        public AgreementToCreateSent(Guid aggregateId, int sequence, Guid sessionId, Guid companyId) : base(aggregateId, sequence, sessionId, companyId)
        {
        }

        protected override string Description => "Convention à créer envoyé";
    }
}