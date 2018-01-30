using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainers.Exceptions
{
    public class ForbiddenDeleteTrainerException : DomainException
    {
        public ForbiddenDeleteTrainerException() : base("Impossible de supprimer ce formateur car celui-ci est associé à une ou plusieurs sessions")
        {            
        }
    }
}