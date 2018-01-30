using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Seats.Exceptions
{
    public class ValidateSeatException : DomainException
    {
        public ValidateSeatException() : base("Vous ne pouvez pas valider cette place si celle ci a déjà fait l'objet d'une modification d'état (annulée ou refusée)")
        {
        }
    }
}
