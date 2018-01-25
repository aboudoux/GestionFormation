using GestionFormation.Kernel;

namespace GestionFormation.Applications.Lieux.Exceptions
{
    public class LieuWithEmptyNameException : DomainException
    {
        public LieuWithEmptyNameException() : base("Le nom du lieu ne doit pas être vide")
        {
        }
    }
}