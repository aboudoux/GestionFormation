using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Exceptions
{
    public class TooManySeatsAlreadyReservedException : DomainException
    {
        public TooManySeatsAlreadyReservedException(int seatAlreadyReserved, int newSiteNumber) : base($"{seatAlreadyReserved} place(s) ont déjà été réservées pour cette session. Vous ne pouvez pas baisser le nombre de place à {newSiteNumber} sans annuler ces places")
        {
            
        }
    }
}
