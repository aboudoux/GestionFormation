using System;
using GestionFormation.Applications.Lieux.Exceptions;
using GestionFormation.CoreDomain.Locations;
using GestionFormation.CoreDomain.Locations.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Lieux
{
    public class CreateLieu : ActionCommand
    {
        private readonly ILocationQueries _locationQueries;

        public CreateLieu(EventBus eventBus, ILocationQueries locationQueries) : base(eventBus)
        {
            _locationQueries = locationQueries ?? throw new ArgumentNullException(nameof(locationQueries));
        }

        public Location Execute(string nom, string addresse, int places)
        {
            if(_locationQueries.GetLocation(nom).HasValue)
                throw new LieuAlreadyExistsException(nom);

            var lieu = Location.Create(nom, addresse, places);
            PublishUncommitedEvents(lieu);
            return lieu;
        }
    }
}
