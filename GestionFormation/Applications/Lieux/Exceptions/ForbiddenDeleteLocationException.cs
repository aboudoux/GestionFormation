using GestionFormation.Kernel;

namespace GestionFormation.Applications.Lieux.Exceptions
{
    public class ForbiddenDeleteLocationException : DomainException
    {
        public ForbiddenDeleteLocationException() : base("Impossible de supprimer ce lieu car celui-ci est associé à une ou plusieurs sessions")
        {
            
        }
    }
}