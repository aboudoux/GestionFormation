using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Contacts.Queries;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure.Contacts.Queries
{
    public class ContactQueries : IContactQueries, IRuntimeDependency
    {
        public IEnumerable<IContactResult> GetAll(Guid companyId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return (from c in context.Contacts
                    where c.CompanyId == companyId && c.Removed == false
                    join company in context.Companies on c.CompanyId equals company.CompanyId into cc
                    from company in cc.DefaultIfEmpty()
                    select new {Contact = c, CompanyName = company == null ? string.Empty : company.Name}
                ).ToList().Select(a => new ContactResult(a.Contact, a.CompanyName));
            }
        }

        public IEnumerable<IContactResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return (from c in context.Contacts
                    where c.Removed == false
                    join company in context.Companies on c.CompanyId equals company.CompanyId into cc
                    from company in cc.DefaultIfEmpty()
                    select new { Contact = c, CompanyName = company == null ? string.Empty : company.Name }
                ).ToList().Select(a => new ContactResult(a.Contact, a.CompanyName));
            }
        }

        public IContactResult GetAgreementContact(Guid agreementId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var querie = from agreement in context.Agreements
                    join contact in context.Contacts on agreement.ContactId equals contact.ContactId
                    where agreement.AgreementId == agreementId
                    select contact;

                var result = querie.FirstOrDefault();
                return result == null ? null : new ContactResult(result, string.Empty);
            }
        }
    }
}