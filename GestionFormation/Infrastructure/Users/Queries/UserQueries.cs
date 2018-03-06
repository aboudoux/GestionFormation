using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Users.Queries;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure.Users.Projections;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure.Users.Queries
{
    public class UserQueries : IUserQueries, IRuntimeDependency
    {
        public IEnumerable<IUserResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Users.ToList().Select(a => new UserResult(a));
            }
        }

        public IUserResult GetLogin(string login, string password)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var encryptedPassword = password.GetHash();
                var result = context.Users.FirstOrDefault(a =>a.Login == login && a.EncryptedPassword == encryptedPassword && a.IsEnabled);
                return result != null ? new UserResult(result) : null;
            }
        }

        public bool Exists(string login)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {             
                return context.Users.Any(a => a.Login == login);
            }
        }

        public IUserResult GetUser(Guid userId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.GetEntity<UserSqlEntity>(userId);
                return new UserResult(entity);
            }
        }
    }
}