using GestionFormation.CoreDomain.Utilisateurs.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Utilisateurs.Projections
{
    public class UtilisateurSqlProjections : IProjectionHandler,
        IEventHandler<UtilisateurCreated>,
        IEventHandler<UtilisateurUpdated>,
        IEventHandler<UtilisateurDeleted>,
        IEventHandler<PasswordChanged>,
        IEventHandler<UtilisateurRoleChanged>


    {
        public void Handle(UtilisateurCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Utilisateurs.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new UtilisateurSqlEntity();
                    context.Utilisateurs.Add(entity);
                }

                entity.Id = @event.AggregateId;
                entity.Login = @event.Login;
                entity.Prenom = @event.Prenom;
                entity.Nom = @event.Nom;
                entity.IsEnabled = true;
                entity.Email = @event.Email;
                entity.EncryptedPassword = @event.EncryptedPassword;
                entity.Role = @event.Role;
                context.SaveChanges();
            }
        }

        public void Handle(UtilisateurUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Utilisateurs.Find(@event.AggregateId);
                if (entity == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Utilisateurs");

                entity.Nom = @event.Nom;
                entity.Prenom = @event.Prenom;
                entity.IsEnabled = @event.IsEnabled;
                entity.Email = @event.Email;
                context.SaveChanges();
            }
        }

        public void Handle(UtilisateurDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new UtilisateurSqlEntity(){ Id = @event.AggregateId };
                context.Utilisateurs.Attach(entity);
                context.Utilisateurs.Remove(entity);
                context.SaveChanges();
            }
        }

        public void Handle(PasswordChanged @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Utilisateurs.Find(@event.AggregateId);
                if (entity == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Utilisateur");

                entity.EncryptedPassword = @event.EncryptedPassword;
                context.SaveChanges();
            }
        }

        public void Handle(UtilisateurRoleChanged @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Utilisateurs.Find(@event.AggregateId);
                if (entity == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Utilisateur");

                entity.Role = @event.Role;
                context.SaveChanges();
            }
        }
    }
}
