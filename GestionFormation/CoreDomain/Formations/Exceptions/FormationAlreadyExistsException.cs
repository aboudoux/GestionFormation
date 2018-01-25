using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formations.Exceptions
{
    public class FormationAlreadyExistsException : DomainException
    {
        public FormationAlreadyExistsException(string name) : base($"Il existe déjà une formation nommée {name}")
        {
            
        }
    }
}
