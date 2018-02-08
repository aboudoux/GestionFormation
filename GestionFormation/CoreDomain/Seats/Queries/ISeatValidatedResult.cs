using System;

namespace GestionFormation.CoreDomain.Seats.Queries
{
    public interface ISeatValidatedResult
    {
        Guid StudentId { get; }
        FullName Student { get; }
        string Company { get; }
        FullName Contact { get; }
        string Telephone { get; }
        string Email { get; }
    }
}