using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formateurs.Exceptions
{
    public class FormateurNameException : DomainException
    {
        public FormateurNameException() : base("Le nom et le prénom du formateur sont obligatoires")
        {
        }
    }
}