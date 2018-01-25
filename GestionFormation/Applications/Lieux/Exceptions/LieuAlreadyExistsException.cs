using GestionFormation.Kernel;

namespace GestionFormation.Applications.Lieux.Exceptions
{
    public class LieuAlreadyExistsException : DomainException
    {
        public LieuAlreadyExistsException(string nom) : base($"Il existe déjà un lieu nommé {nom} dans la base de données.")
        {
            
        }
    }
}