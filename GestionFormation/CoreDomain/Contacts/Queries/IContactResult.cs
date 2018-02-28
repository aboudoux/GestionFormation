using System;

namespace GestionFormation.CoreDomain.Contacts.Queries
{
    public interface IContactResult
    {
        Guid Id { get; }
        string Lastname { get; }
        string Firstname { get; }
        string Email { get; }
        string Telephone { get; }
        Guid CompanyId { get; }
        string CompanyName { get; }
    }
}