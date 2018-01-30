using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainers.Exceptions
{
    public class TrainerAlreadyAssignedException : DomainException
    {
        public TrainerAlreadyAssignedException() : base("Le formateur est déjà assigné à une autre session dans la plage sélectionnée.")
        {
            
        }
    }
}