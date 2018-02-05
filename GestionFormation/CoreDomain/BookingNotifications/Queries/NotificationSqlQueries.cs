using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Users;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.BookingNotifications.Queries
{
    public class BookingNotificationQueries : INotificationQueries, IRuntimeDependency
    {
        public IEnumerable<INotificationResult> GetAll(UserRole role)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                if( role == UserRole.Admin)
                    return context.BookingNotifications.ToList().Select(a => new NotificationResult(a));                
                return context.BookingNotifications.Where(a => a.AffectedRole == role).ToList().Select(a => new NotificationResult(a));
            }
        }

        public IEnumerable<INotificationResult> GetAll(Guid sessionId, Guid companyId)
        {
            sessionId.EnsureNotEmpty(nameof(sessionId));
            companyId.EnsureNotEmpty(nameof(companyId));

            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.BookingNotifications.Where(a => a.SessionId == sessionId && a.CompanyId == companyId).ToList().Select(a => new NotificationResult(a));
            }
        }

        public IEnumerable<INotificationResult> GetFromAgreement(Guid agreementId)
        {
            agreementId.EnsureNotEmpty(nameof(agreementId));

            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.BookingNotifications.Where(a => a.AgreementId == agreementId).ToList().Select(a => new NotificationResult(a));
            }
        }

        public IEnumerable<INotificationResult> GetFromSeat(Guid seatId)
        {
            seatId.EnsureNotEmpty(nameof(seatId));
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.BookingNotifications.Where(a => a.SeatId == seatId).ToList().Select(a => new NotificationResult(a));
            }
        }
    }
}