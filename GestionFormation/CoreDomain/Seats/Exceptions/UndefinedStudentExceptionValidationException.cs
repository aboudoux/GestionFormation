using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Seats.Exceptions
{
    public class UndefinedStudentExceptionValidationException : DomainException
    {
        public UndefinedStudentExceptionValidationException() : base("Vous ne pouvez pas valider une place lorsque le stagiaire n'a pas été défini")
        {
            
        }
    }
}