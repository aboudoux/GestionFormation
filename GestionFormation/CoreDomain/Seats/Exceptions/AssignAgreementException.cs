using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Seats.Exceptions
{
    public class AssignAgreementException : DomainException
    {
        public AssignAgreementException() : base("Vous ne pouvez pas assigner une convention à une place qui n'est pas validée")
        {
        }
    }
}