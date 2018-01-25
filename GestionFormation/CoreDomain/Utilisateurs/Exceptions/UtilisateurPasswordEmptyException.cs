using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Utilisateurs.Exceptions
{
    public class UtilisateurPasswordEmptyException : DomainException
    {
        public UtilisateurPasswordEmptyException() : base("Le mot de passe ne doit pas être vide")
        {
        }
    }
}