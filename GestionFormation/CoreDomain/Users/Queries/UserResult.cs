using System;
using GestionFormation.CoreDomain.Users.Projections;

namespace GestionFormation.CoreDomain.Users.Queries
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
        }
        public Guid Id { get; }
        public string Login { get; }
        public string Lastname { get; }
        public string Firsname { get; }
        public bool IsEnabled { get; }
        public string Email { get; }
        public UserRole Role { get; }
    }
}