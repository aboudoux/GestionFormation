using System;
using System.Collections.Generic;
using System.Text;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Conventions.Exceptions
{
    public class CannotSignRevokedConvention : DomainException
    {
        public CannotSignRevokedConvention() : base("Vous ne pouvez pas signer une convention révoquée")
        {
        }
    }
}
