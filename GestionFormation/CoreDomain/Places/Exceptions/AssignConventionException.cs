using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Places.Exceptions
{
    public class AssignConventionException : DomainException
    {
        public AssignConventionException() : base("Vous ne pouvez pas assigner une convention à une place qui n'est pas validée")
        {
        }
    }
}