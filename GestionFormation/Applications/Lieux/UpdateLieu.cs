using System;
using GestionFormation.Applications.Lieux.Exceptions;
using GestionFormation.CoreDomain.Locations;
using GestionFormation.CoreDomain.Locations.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Lieux
{
    public class UpdateLieu : ActionCommand
    {
        private readonly ILocationQueries _locationQueries;

        public UpdateLieu(EventBus eventBus, ILocationQueries locationQueries) : base(eventBus)
        {
            _locationQueries = locationQueries ?? throw new ArgumentNullException(nameof(locationQueries));
        }

        public void Execute(Guid lieuId, string newNom, string addresse, int places)
        {
            var foundLieuId = _locationQueries.GetLocation(newNom);

            if (foundLieuId.HasValue && foundLieuId.Value != lieuId)
                throw new LieuAlreadyExistsException(newNom);

            var lieu = GetAggregate<Location>(lieuId);
            lieu.Update(newNom, addresse, places);
            PublishUncommitedEvents(lieu);
        }
    }
}