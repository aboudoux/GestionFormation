using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formateurs.Exceptions
{
    public class ForbiddenDeleteFormateurException : DomainException
    {
        public ForbiddenDeleteFormateurException() : base("Impossible de supprimer ce formateur car celui-ci est associé à une ou plusieurs sessions")
        {            
        }
    }
}