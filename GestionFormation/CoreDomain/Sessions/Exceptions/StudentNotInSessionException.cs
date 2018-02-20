using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Exceptions
{
    public class StudentNotInSessionException : DomainException
    {
        public StudentNotInSessionException() : base("Le stagiaire n'est pas défini pour cette session")
        {
            
        }
    }
}