using GestionFormation.CoreDomain.Users.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Users.Projections
{
    public class UserSqlProjections : IProjectionHandler,
        IEventHandler<UserCreated>,
        IEventHandler<UserUpdated>,
        IEventHandler<UserDeleted>,
        IEventHandler<PasswordChanged>,
        IEventHandler<UserRoleChanged>
    {
        public void Handle(UserCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Users.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new UserSqlEntity();
                    context.Users.Add(entity);
                }

                entity.Id = @event.AggregateId;
                entity.Login = @event.Login;
                entity.Firstname = @event.Firstname;
                entity.Lastname = @event.Lastname;
                entity.IsEnabled = true;
                entity.Email = @event.Email;
                entity.EncryptedPassword = @event.EncryptedPassword;
                entity.Role = @event.Role;
                context.SaveChanges();
            }
        }

        public void Handle(UserUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Users.Find(@event.AggregateId);
                if (entity == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Utilisateurs");

                entity.Lastname = @event.Lastname;
                entity.Firstname = @event.Firstname;
                entity.IsEnabled = @event.IsEnabled;
                entity.Email = @event.Email;
                context.SaveChanges();
            }
        }

        public void Handle(UserDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new UserSqlEntity(){ Id = @event.AggregateId };
                context.Users.Attach(entity);
                context.Users.Remove(entity);
                context.SaveChanges();
            }
        }

        public void Handle(PasswordChanged @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Users.Find(@event.AggregateId);
                if (entity == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Utilisateur");

                entity.EncryptedPassword = @event.EncryptedPassword;
                context.SaveChanges();
            }
        }

        public void Handle(UserRoleChanged @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Users.Find(@event.AggregateId);
                if (entity == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Utilisateur");

                entity.Role = @event.Role;
                context.SaveChanges();
            }
        }
    }
}
