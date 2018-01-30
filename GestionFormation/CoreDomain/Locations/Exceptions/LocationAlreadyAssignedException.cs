using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Locations.Exceptions
{
    public class LocationAlreadyAssignedException : DomainException
    {
        public LocationAlreadyAssignedException() : base("Le lieu est déjà assigné à une autre session dans la plage sélectionnée.")
        {            
        }
    }
}