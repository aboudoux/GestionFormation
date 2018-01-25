using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Exceptions
{
    public class NoMorePlacesAvailableException : DomainException
    {
        public NoMorePlacesAvailableException() : base("Il n'y a plus de place disponible pour cette session.")
        {
            
        }
    }
}