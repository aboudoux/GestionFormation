using GestionFormation.Kernel;

namespace GestionFormation.Applications.Locations.Exceptions
{
    public class LocationAlreadyExistsException : DomainException
    {
        public LocationAlreadyExistsException(string name) : base($"Il existe déjà un lieu nommé {name} dans la base de données.")
        {
            
        }
    }
}