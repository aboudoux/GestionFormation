using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Lieux.Exceptions
{
    public class LieuAlreadyAssignedException : DomainException
    {
        public LieuAlreadyAssignedException() : base("Le lieu est déjà assigné à une autre session dans la plage sélectionnée.")
        {            
        }
    }
}