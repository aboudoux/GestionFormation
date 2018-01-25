using System;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Exceptions
{
    public class TooManyPlacesAlreadyReservedException : DomainException
    {
        public TooManyPlacesAlreadyReservedException(int placeDejaReservées, int nouveauNombrePlaces) : base($"{placeDejaReservées} place(s) ont déjà été réservées pour cette session. Vous ne pouvez pas baisser le nombre de place à {nouveauNombrePlaces} sans annuler ces places")
        {
            
        }
    }
}
