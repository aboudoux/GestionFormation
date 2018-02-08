using System;
using GestionFormation.Applications.Locations.Exceptions;
using GestionFormation.CoreDomain.Locations;
using GestionFormation.CoreDomain.Locations.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Locations
{
    public class UpdateLocation : ActionCommand
    {
        private readonly ILocationQueries _locationQueries;

        public UpdateLocation(EventBus eventBus, ILocationQueries locationQueries) : base(eventBus)
        {
            _locationQueries = locationQueries ?? throw new ArgumentNullException(nameof(locationQueries));
        }

        public void Execute(Guid locationId, string newName, string address, int seats)
        {
            var foundLocationId = _locationQueries.GetLocation(newName);

            if (foundLocationId.HasValue && foundLocationId.Value != locationId)
                throw new LocationAlreadyExistsException(newName);

            var location = GetAggregate<Location>(locationId);
            location.Update(newName, address, seats);
            PublishUncommitedEvents(location);
        }
    }
}