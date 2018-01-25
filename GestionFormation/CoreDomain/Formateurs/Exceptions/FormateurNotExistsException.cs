using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formateurs.Exceptions
{
    public class FormateurNotExistsException : DomainException
    {
        public FormateurNotExistsException() : base("Le formateur que vous essayez de charger n'existe pas")
        {
            
        }
    }
}