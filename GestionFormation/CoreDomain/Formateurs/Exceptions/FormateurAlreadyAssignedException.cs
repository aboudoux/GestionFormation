using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formateurs.Exceptions
{
    public class FormateurAlreadyAssignedException : DomainException
    {
        public FormateurAlreadyAssignedException() : base("Le formateur est déjà assigné à une autre session dans la plage sélectionnée.")
        {
            
        }
    }
}