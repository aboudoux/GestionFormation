using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formations.Exceptions
{
    public class FormationEmptyNameException : DomainException
    {
        public FormationEmptyNameException() : base("Le nom de la formation est vide")
        {
            
        }
    }
}