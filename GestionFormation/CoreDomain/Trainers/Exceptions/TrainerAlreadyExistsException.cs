using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainers.Exceptions
{
    public class TrainerAlreadyExistsException : DomainException
    {
        public TrainerAlreadyExistsException() : base("Le formateur existe déjà dans la base de données")
        {
            
        }
    }
}