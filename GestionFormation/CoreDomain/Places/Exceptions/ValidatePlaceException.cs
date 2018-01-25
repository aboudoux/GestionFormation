using System;
using System.Collections.Generic;
using System.Text;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Places.Exceptions
{
    public class ValidatePlaceException : DomainException
    {
        public ValidatePlaceException() : base("Vous ne pouvez pas valider cette place si celle ci a déjà fait l'objet d'une modification d'état (annulée ou refusée)")
        {
        }
    }
}
