using System;
using GestionFormation.CoreDomain.Contacts.Queries;
using GestionFormation.Infrastructure.Contacts.Projections;

namespace GestionFormation.Infrastructure.Contacts.Queries
{
    public class ContactResult : IContactResult
    {
        public ContactResult(ContactSqlEntity entity, string companyName)
        {
            Id = entity.ContactId;
            Lastname = entity.Lastname;
            Firstname = entity.Firstname;
            Email = entity.Email;
            Telephone = entity.Telephone;
            CompanyId = entity.CompanyId;
            CompanyName = companyName;
        }
        public Guid Id { get; }
        public string Lastname { get; }
        public string Firstname { get; }
        public string Email { get; }
        public string Telephone { get; }
        public Guid CompanyId { get; }
        public string CompanyName { get; }
    }
}