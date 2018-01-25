using System;
using GestionFormation.CoreDomain.Contacts.Projections;

namespace GestionFormation.CoreDomain.Contacts.Queries
{
    public class ContactResult : IContactResult
    {
        public ContactResult(ContactSqlEntity entity)
        {
            Id = entity.ContactId;
            Nom = entity.Nom;
            Prenom = entity.Prenom;
            Email = entity.Email;
            Telephone = entity.Telephone;
        }
        public Guid Id { get; }
        public string Nom { get; }
        public string Prenom { get; }
        public string Email { get; }
        public string Telephone { get; }
    }
}