using System;
using System.Collections.Generic;
using GestionFormation.CoreDomain.Notifications.Queries;
using GestionFormation.CoreDomain.Users;

namespace GestionFormation.Tests.Fakes
{
    public class FakeNotificationQueries : INotificationQueries
    {
        private readonly Dictionary<Guid, Guid> _notificationManagers = new Dictionary<Guid, Guid>();

        public void AddNotificationManager(Guid sessionId, Guid notificationManagerId)
        {
            _notificationManagers.Add(sessionId, notificationManagerId);
        }

        public IEnumerable<INotificationResult> GetAll(UserRole role)
        {
            throw new NotImplementedException();
        }

        public Guid GetNotificationManagerId(Guid sessionId)
        {
            return _notificationManagers[sessionId];
        }

        public Guid GetNotificationManagerIdFromAgreement(Guid agreementId)
        {
            throw new NotImplementedException();
        }
    }
}