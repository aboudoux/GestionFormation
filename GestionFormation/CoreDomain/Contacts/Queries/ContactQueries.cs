using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Contacts.Queries
{
    public class ContactQueries : IContactQueries, IRuntimeDependency
    {
        public IEnumerable<IContactResult> GetAll(Guid companyId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Contacts.Where(a=>a.CompanyId == companyId && a.Removed == false).ToList().Select(a => new ContactResult(a));
            }
        }

        public IEnumerable<IContactResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Contacts.Where(a=>a.Removed == false).ToList().Select(a => new ContactResult(a));
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
                return result == null ? null : new ContactResult(result);
            }
        }
    }
}