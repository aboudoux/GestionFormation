using System;
using System.Collections.Generic;
using System.Text;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Contacts.Exceptions
{
    public class CreateContactException : DomainException
    {
        public CreateContactException() : base("Impossible de créer le contact car la société à laquele vous voulez le rattacher n'existe pas")
        {
            
        }
    }
}
