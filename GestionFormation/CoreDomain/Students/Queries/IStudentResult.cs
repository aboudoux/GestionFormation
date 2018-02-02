using System;

namespace GestionFormation.CoreDomain.Students.Queries
{
    public interface IStudentResult
    {
        Guid Id { get; }
        string Lastname { get; }
        string Firstname { get; }
    }
}