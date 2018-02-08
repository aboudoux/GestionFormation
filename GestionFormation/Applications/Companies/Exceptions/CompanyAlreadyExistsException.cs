using System;
using System.Collections.Generic;
using System.Text;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Companies.Exceptions
{
    public class CompanyAlreadyExistsException : DomainException
    {
        public CompanyAlreadyExistsException(string name) : base($"Il existe déjà une société nommée {name} dans la base de données.")
        {
            
        }
    }
}
