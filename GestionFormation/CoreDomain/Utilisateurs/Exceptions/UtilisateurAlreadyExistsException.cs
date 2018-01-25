using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Utilisateurs
{
    public class UtilisateurAlreadyExistsException : DomainException
    {
        public UtilisateurAlreadyExistsException() : base("L'utilisateur existe déjà")
        {
        }
    }
}