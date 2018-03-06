using System;
using GestionFormation.CoreDomain.Users;
using GestionFormation.CoreDomain.Users.Exceptions;
using GestionFormation.CoreDomain.Users.Queries;
using GestionFormation.Kernel;

namespace GestionFormation.Applications.Users
{
    public class CreateUser : ActionCommand
    {
        private readonly IUserQueries _userQueries;

        public CreateUser(EventBus eventBus, IUserQueries userQueries) : base(eventBus)
        {
            _userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
        }

        public void Execute(string login, string password, string lastname, string firstname, string email, UserRole role, string signature)
        {
            if(_userQueries.Exists(login))
                throw new UserAlreadyExistsException();

            var user = User.Create(login, password, lastname, firstname, email, role, signature);
            PublishUncommitedEvents(user);
        }
    }
}
