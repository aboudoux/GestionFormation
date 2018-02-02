using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Users.Exceptions
{
    public class UserAlreadyExistsException : DomainException
    {
        public UserAlreadyExistsException() : base("L'utilisateur existe déjà")
        {
        }
    }
}