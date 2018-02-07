using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Agreements.Queries;
using GestionFormation.CoreDomain.Notifications;
using GestionFormation.CoreDomain.Notifications.Queries;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Agreements
{
    public class CreateAgreement : ActionCommand
    {
        private readonly IAgreementQueries _agreementQueries;
        private readonly INotificationQueries _notificationQueries;

        public CreateAgreement(EventBus eventBus, IAgreementQueries agreementQueries, INotificationQueries notificationQueries) : base(eventBus)
        {
            _agreementQueries = agreementQueries ?? throw new ArgumentNullException(nameof(agreementQueries));
            _notificationQueries = notificationQueries ?? throw new ArgumentNullException(nameof(notificationQueries));
        }

        public Agreement Execute(Guid contactId, IEnumerable<Guid> seatsIds, AgreementType agreementType)
        {
            CheckThereAreNoDuplicate(seatsIds);
            var aggregatesToCommit = new List<AggregateRoot>();
            Guid? companyId = null;

            var agreementNumber = _agreementQueries.GetNextAgreementNumber();
            if( agreementNumber <= 0)
                throw new Exception("Impossible d'obtenir le numéro de convention.");

            var agreement = Agreement.Create(contactId, agreementNumber, agreementType);
            aggregatesToCommit.Add(agreement);

            NotificationManager manager = null;

            foreach (var seatId in seatsIds)
            {
                var seat = GetAggregate<Seat>(seatId);
                if (manager == null)
                {
                    var managerId = _notificationQueries.GetNotificationManagerId(seat.SessionId);
                    manager = GetAggregate<NotificationManager>(managerId);                    
                    aggregatesToCommit.Add(manager);
                }

                if (companyId.HasValue && companyId.Value != seat.CompanyId)
                    throw new AgreementCompanyException();
                companyId = seat.CompanyId;

                seat.AssociateAgreement(agreement.AggregateId);
                manager.SignalAgreementAssociated(agreement.AggregateId, seat.AggregateId, seat.CompanyId);

                aggregatesToCommit.Add(seat);
            }

            PublishUncommitedEvents(aggregatesToCommit.ToArray());
            return agreement;
        }

        private static void CheckThereAreNoDuplicate(IEnumerable<Guid> seatsIds)
        {
            var hashSet = new HashSet<Guid>();
            if (seatsIds.Any(seatId => !hashSet.Add(seatId)))
                throw new ArgumentException("La liste des places contient un identifiant en double");
        }
    }

    public class AgreementCompanyException : DomainException
    {
        public AgreementCompanyException() : base("Une convention ne peut pas être créée pour des sociétés différentes.")
        {
            
        }
    }
}
