using System;

namespace GestionFormation.CoreDomain.Contacts.Queries
{
    public interface IContactResult
    {
        Guid Id { get; }
        string Nom { get; }
        string Prenom { get; }
        string Email { get; }
        string Telephone { get; }
    }
}