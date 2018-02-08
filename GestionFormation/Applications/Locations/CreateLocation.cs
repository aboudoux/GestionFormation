using System;
using GestionFormation.Applications.Locations.Exceptions;
using GestionFormation.CoreDomain.Locations;
using GestionFormation.CoreDomain.Locations.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Locations
{
    public class CreateLocation : ActionCommand
    {
        private readonly ILocationQueries _locationQueries;

        public CreateLocation(EventBus eventBus, ILocationQueries locationQueries) : base(eventBus)
        {
            _locationQueries = locationQueries ?? throw new ArgumentNullException(nameof(locationQueries));
        }

        public Location Execute(string name, string address, int seats)
        {
            if(_locationQueries.GetLocation(name).HasValue)
                throw new LocationAlreadyExistsException(name);

            var location = Location.Create(name, address, seats);
            PublishUncommitedEvents(location);
            return location;
        }
    }
}
