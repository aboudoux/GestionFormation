using System;
using System.Collections.Generic;
using System.Text;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.BookingNotifications.Exceptions
{
    public class ChangeNotificationException : DomainException
    {
        public ChangeNotificationException() : base("Impossible de changer la notification de statut car elle n'est aps dans le bon état inital")
        {
            
        }
    }
}
