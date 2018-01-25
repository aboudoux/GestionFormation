using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Lieux.Exceptions
{
    public class LieuNotExistsException : DomainException
    {
        public LieuNotExistsException() : base("Le lieu que vous essayez de charger n'existe pas")
        {
            
        }
    }
}