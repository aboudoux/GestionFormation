using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainings.Exceptions
{
    public class TrainingAlreadyExistsException : DomainException
    {
        public TrainingAlreadyExistsException(string name) : base($"Il existe déjà une formation nommée {name}")
        {
            
        }
    }
}
