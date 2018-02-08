using GestionFormation.Kernel;

namespace GestionFormation.Applications.Locations.Exceptions
{
    public class LocationWithEmptyNameException : DomainException
    {
        public LocationWithEmptyNameException() : base("Le nom du lieu ne doit pas être vide")
        {
        }
    }
}