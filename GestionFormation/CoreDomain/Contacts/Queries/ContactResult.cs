using System;
using GestionFormation.CoreDomain.Contacts.Projections;

namespace GestionFormation.CoreDomain.Contacts.Queries
{
    public class ContactResult : IContactResult
    {
        public ContactResult(ContactSqlEntity entity)
        {
            Id = entity.ContactId;
            Lastname = entity.Lastname;
            Firstname = entity.Firstname;
            Email = entity.Email;
            Telephone = entity.Telephone;
            CompanyId = entity.CompanyId;
        }
        public Guid Id { get; }
        public string Lastname { get; }
        public string Firstname { get; }
        public string Email { get; }
        public string Telephone { get; }
        public Guid CompanyId { get; }
    }
}