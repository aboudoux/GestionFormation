using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainings.Exceptions
{
    public class TrainingEmptyNameException : DomainException
    {
        public TrainingEmptyNameException() : base("Le nom de la formation est vide")
        {
            
        }
    }
}