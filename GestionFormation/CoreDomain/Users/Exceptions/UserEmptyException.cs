using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Users.Exceptions
{
    public class UserEmptyException : DomainException
    {
        public UserEmptyException(string emptyMember) : base($"{emptyMember} ne doit pas être vide")
        {
        }
    }
}