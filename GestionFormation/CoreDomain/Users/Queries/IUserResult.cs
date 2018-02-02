using System;

namespace GestionFormation.CoreDomain.Users.Queries
{
    public interface IUserResult
    {
        Guid Id { get; }
        string Login { get; }
        string Lastname { get; }
        string Firsname { get; }
        bool IsEnabled { get; }
        string Email { get; }
        UserRole Role { get; }
    }
}