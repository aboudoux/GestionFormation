using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Conventions;
using GestionFormation.CoreDomain.Conventions.Queries;
using GestionFormation.CoreDomain.Places;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Conventions
{
    public class CreateConvention : ActionCommand
    {
        private readonly IConventionQueries _conventionQueries;

        public CreateConvention(EventBus eventBus, IConventionQueries conventionQueries) : base(eventBus)
        {
            _conventionQueries = conventionQueries ?? throw new ArgumentNullException(nameof(conventionQueries));
        }

        public Convention Execute(Guid contactId, IEnumerable<Guid> placesIds, TypeConvention typeConvention)
        {
            CheckThereAreNoDuplicate(placesIds);
            var aggregatesToCommit = new List<AggregateRoot>();
            Guid? societeId = null;

            var numeroConvention = _conventionQueries.GetNextConventionNumber();
            if( numeroConvention <= 0)
                throw new Exception("Impossible d'obtenir le numéro de convention.");

            var convention = Convention.Create(contactId, numeroConvention, typeConvention);
            aggregatesToCommit.Add(convention);

            foreach (var placeId in placesIds)
            {
                var place = GetAggregate<Place>(placeId);
                if (societeId.HasValue && societeId.Value != place.SocieteId)
                    throw new ConventionSocieteException();
                societeId = place.SocieteId;

                place.AssociateConvention(convention.AggregateId);
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
