using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Conventions.Queries
{
    public class ConventionQueries : IConventionQueries, IRuntimeDependency
    {
        public IEnumerable<IConventionResult> GetAll(Guid sessionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var query = from places in context.Places
                    where places.SessionId == sessionId
                    join convention in context.Conventions on places.AssociatedConventionId equals convention.ConventionId
                    join societe in context.Societes on places.SocieteId equals societe.SocieteId
                    join contact in context.Contacts on convention.ContactId equals contact.ContactId                    
                    select new {Societe = societe.Nom, Contact = contact.Nom, convention.ConventionId, places.PlaceId, convention.ConventionNumber};

                return query.ToList().GroupBy(g => g.ConventionId, (key, a) =>
                {
                    var item = a.First();
                    return new ConventionResult(item.ConventionId, item.Societe, item.Contact, item.ConventionNumber, a.Select(b => b.PlaceId).ToList());
                });
            }
        }

        public long GetNextConventionNumber()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Database.SqlQuery<long>("select next value for CompteurConvention").First();
            }
        }
    }
}