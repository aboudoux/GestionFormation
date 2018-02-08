using System;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Notifications;
using GestionFormation.CoreDomain.Notifications.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Agreements
{
    public class SignAgreement : ActionCommand
    {
        private readonly INotificationQueries _notificationQueries;

        public SignAgreement(EventBus eventBus, INotificationQueries notificationQueries) : base(eventBus)
        {
            _notificationQueries = notificationQueries ?? throw new ArgumentNullException(nameof(notificationQueries));
        }

        public void Execute(Guid agreementId, Guid documentId)
        {
            var agreement = GetAggregate<Agreement>(agreementId);
            agreement.Sign(documentId);

            var notifId = _notificationQueries.GetNotificationManagerIdFromAgreement(agreementId);
            var notification = GetAggregate<NotificationManager>(notifId);
            notification.SignalAgreementSigned(agreementId);

            PublishUncommitedEvents(agreement, notification);
        }
    }
}