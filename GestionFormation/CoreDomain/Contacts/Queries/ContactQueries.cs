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
        public IEnumerable<IContactResult> GetAll(Guid societeId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Contacts.Where(a=>a.SocieteId == societeId).ToList().Select(a => new ContactResult(a));
            }
        }

        public IEnumerable<IContactResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Contacts.ToList().Select(a => new ContactResult(a));
            }
        }

        public IContactResult GetConventionContact(Guid conventionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var querie = from conv in context.Conventions
                    join contact in context.Contacts on conv.ContactId equals contact.ContactId
                    where conv.ConventionId == conventionId
                    select contact;

                var result = querie.FirstOrDefault();
                return result == null ? null : new ContactResult(result);
            }
        }
    }
}