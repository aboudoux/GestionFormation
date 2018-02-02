using System;
using GestionFormation.CoreDomain.Users.Events;
using GestionFormation.CoreDomain.Users.Exceptions;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Users
{
    public class User : AggregateRootUpdatableAndDeletable<UserUpdated, UserDeleted>
    {
        private UserRole _currentRole;

        public User(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            base.AddPlayers(player);
            player.Add<UserCreated>(e => _currentRole = e.Role);
        }

        public static User Create(string login, string password, string lastname, string firstname, string email, UserRole role)
        {
            if(string.IsNullOrWhiteSpace(login))
                throw new UserEmptyException("Le login");
            if(string.IsNullOrWhiteSpace(password))
                throw new UserEmptyException("Le mot de passe");
            if(string.IsNullOrWhiteSpace(lastname))
                throw new UserEmptyException("Le lastname");

            var user = new User(History.Empty);
            user.AggregateId = Guid.NewGuid();
            
            user.UncommitedEvents.Add(new UserCreated(user.AggregateId, 1, login, password.GetHash(), lastname, firstname, email, role));
            return user;
        }

        public void Update(string lastname, string firstname, string email, bool isEnabled)
        {                     
            if (string.IsNullOrWhiteSpace(lastname))
                throw new UserEmptyException("Le lastname");

            Update(new UserUpdated(AggregateId, GetNextSequence(), lastname, firstname, email, isEnabled));
        }

        public void Delete()
        {
            Delete(new UserDeleted(AggregateId, GetNextSequence()));
        }

        public void ChangePassword(string newPassword)
        {
            if(string.IsNullOrWhiteSpace(newPassword))
                throw new UserEmptyException("Le mot de passe");

            RaiseEvent(new PasswordChanged(AggregateId, GetNextSequence(), newPassword.GetHash()));
        }

        public void ChangeRole(UserRole role)
        {
            if(_currentRole != role)
                RaiseEvent(new UserRoleChanged(AggregateId, GetNextSequence(), role));
        }
    }
}
