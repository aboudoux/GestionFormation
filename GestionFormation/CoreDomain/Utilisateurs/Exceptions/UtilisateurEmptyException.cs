using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Utilisateurs.Exceptions
{
    public class UtilisateurEmptyException : DomainException
    {
        public UtilisateurEmptyException(string emptyMember) : base($"{emptyMember} ne doit pas être vide")
        {
        }
    }
}