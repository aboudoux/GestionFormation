using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Exceptions
{
    public class NoMoreSeatAvailableException : DomainException
    {
        public NoMoreSeatAvailableException() : base("Il n'y a plus de place disponible pour cette session.")
        {
            
        }
    }
}