using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainers.Exceptions
{
    public class TrainerNotExistsException : DomainException
    {
        public TrainerNotExistsException() : base("Le formateur que vous essayez de charger n'existe pas")
        {
            
        }
    }
}