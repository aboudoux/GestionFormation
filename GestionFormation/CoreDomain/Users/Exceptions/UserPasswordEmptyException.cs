using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Users.Exceptions
{
    public class UserPasswordEmptyException : DomainException
    {
        public UserPasswordEmptyException() : base("Le mot de passe ne doit pas être vide")
        {
        }
    }
}