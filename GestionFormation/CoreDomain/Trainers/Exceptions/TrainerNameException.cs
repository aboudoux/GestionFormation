using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainers.Exceptions
{
    public class TrainerNameException : DomainException
    {
        public TrainerNameException() : base("Le nom et le prénom du formateur sont obligatoires")
        {
        }
    }
}