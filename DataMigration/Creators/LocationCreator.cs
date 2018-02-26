using GestionFormation.Applications.Locations;

namespace DataMigration.Creators
{
    public class LocationCreator : Creator
    {
        public LocationCreator(ApplicationService applicationService) : base(applicationService)
        {
        }

        public void Create(string locationName)
        {
            if (Mapper.Exists(locationName)) return;

            var location = App.Command<CreateLocation>().Execute(locationName, "", 8);
            Mapper.Add(locationName, location.AggregateId);
        }
    }
}