using GestionFormation.Kernel;

namespace GestionFormation.Applications.Lieux.Exceptions
{
    public class ForbiddenDeleteLieuException : DomainException
    {
        public ForbiddenDeleteLieuException() : base("Impossible de supprimer ce lieu car celui-ci est associé à une ou plusieurs sessions")
        {
            
        }
    }
}