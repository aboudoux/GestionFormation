using System;
using GestionFormation.CoreDomain.Users;
using GestionFormation.CoreDomain.Users.Queries;
using GestionFormation.Infrastructure.Users.Projections;

namespace GestionFormation.Infrastructure.Users.Queries
{
    public class UserResult : IUserResult
    {
        public UserResult(UserSqlEntity entity)
        {
            Id = entity.Id;
            Login = entity.Login;
            Lastname = entity.Lastname;
            Firsname = entity.Firstname;
            Email = entity.Email;
            IsEnabled = entity.IsEnabled;
            Role = entity.Role;
            Signature = entity.Signature;
        }
        public Guid Id { get; }
        public string Login { get; }
        public string Lastname { get; }
        public string Firsname { get; }
        public bool IsEnabled { get; }
        public string Email { get; }
        public UserRole Role { get; }
        public string Signature { get; }
    }
}