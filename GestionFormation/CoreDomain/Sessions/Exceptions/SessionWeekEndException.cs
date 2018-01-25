using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Exceptions
{
    public class SessionWeekEndException : DomainException
    {
        public SessionWeekEndException() : base("La periode de cette session possède des jours de week-end")
        {
            
        }
    }
}