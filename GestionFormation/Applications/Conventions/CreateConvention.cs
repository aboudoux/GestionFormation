using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Agreements;
using GestionFormation.CoreDomain.Agreements.Queries;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Conventions
{
    public class CreateConvention : ActionCommand
    {
        private readonly IAgreementQueries _agreementQueries;

        public CreateConvention(EventBus eventBus, IAgreementQueries agreementQueries) : base(eventBus)
        {
            _agreementQueries = agreementQueries ?? throw new ArgumentNullException(nameof(agreementQueries));
        }

        public Agreement Execute(Guid contactId, IEnumerable<Guid> placesIds, AgreementType agreementType)
        {
            CheckThereAreNoDuplicate(placesIds);
            var aggregatesToCommit = new List<AggregateRoot>();
            Guid? societeId = null;

            var numeroConvention = _agreementQueries.GetNextAgreementNumber();
            if( numeroConvention <= 0)
                throw new Exception("Impossible d'obtenir le numéro de convention.");

            var convention = Agreement.Create(contactId, numeroConvention, agreementType);
            aggregatesToCommit.Add(convention);

            foreach (var placeId in placesIds)
            {
                var place = GetAggregate<Seat>(placeId);
                if (societeId.HasValue && societeId.Value != place.CompanyId)
                    throw new ConventionSocieteException();
                societeId = place.CompanyId;

                place.AssociateAgreement(convention.AggregateId);
                aggregatesToCommit.Add(place);
            }

            PublishUncommitedEvents(aggregatesToCommit.ToArray());
            return convention;
        }

        private static void CheckThereAreNoDuplicate(IEnumerable<Guid> placesIds)
        {
            var hashSet = new HashSet<Guid>();
            if (placesIds.Any(placeId => !hashSet.Add(placeId)))
                throw new ArgumentException("La liste des places contient un identifiant en double");
        }
    }

    public class ConventionSocieteException : DomainException
    {
        public ConventionSocieteException() : base("Une convention ne peut pas être créée pour des sociétés différentes.")
        {
            
        }
    }
}
