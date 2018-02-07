using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Users;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Notifications.Queries
{
    public class BookingNotificationQueries : INotificationQueries, IRuntimeDependency
    {
        public IEnumerable<INotificationResult> GetAll(UserRole role)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                if( role == UserRole.Admin)
                    return context.Notifications.ToList().Select(a => new NotificationResult(a));                
                return context.Notifications.Where(a => a.AffectedRole == role).ToList().Select(a => new NotificationResult(a));
            }
        }

        public Guid GetNotificationManagerId(Guid sessionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.NotificationManagers.First(a => a.SessionId == sessionId).Id;
            }
        }
    }
}