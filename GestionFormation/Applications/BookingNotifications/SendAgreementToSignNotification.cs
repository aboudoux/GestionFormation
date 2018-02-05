using System;
using System.Linq;
using GestionFormation.CoreDomain.BookingNotifications;
using GestionFormation.CoreDomain.BookingNotifications.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.BookingNotifications
{
    public class SendAgreementToSignNotification : ActionCommand
    {
        private readonly INotificationQueries _notificationQueries;

        public SendAgreementToSignNotification(EventBus eventBus, INotificationQueries notificationQueries) : base(eventBus)
        {
            _notificationQueries = notificationQueries ?? throw new ArgumentNullException(nameof(notificationQueries));
        }

        public void Execute(Guid sessionId, Guid companyId, Guid agreementId)
        {
            agreementId.EnsureNotEmpty(nameof(agreementId));

            var agreementNotification = _notificationQueries.GetAll(sessionId, companyId);

            if(!agreementNotification.Any())
                throw new Exception("agreement not found");
            if(agreementNotification.Count() > 1)
                throw new Exception("Erreur : il existe plus d'une notification pour la convention");

            var notif = GetAggregate<BookingNotification>(agreementNotification.First().AggregateId);
            notif.ChangeToAgreementToSign(agreementId);

            PublishUncommitedEvents(notif);
        }
    }
}