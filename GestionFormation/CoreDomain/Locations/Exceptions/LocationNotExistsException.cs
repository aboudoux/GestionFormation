using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Locations.Exceptions
{
    public class LocationNotExistsException : DomainException
    {
        public LocationNotExistsException() : base("Le lieu que vous essayez de charger n'existe pas")
        {
            
        }
    }
}