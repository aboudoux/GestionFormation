using System;

namespace GestionFormation.CoreDomain.Trainers.Queries
{
    public interface ITrainerResult
    {
        Guid Id { get; }
        string Lastname { get; }
        string Firstname { get; }
        string Email { get; }
    }
}