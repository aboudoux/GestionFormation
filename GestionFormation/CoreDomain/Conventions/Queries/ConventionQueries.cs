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

        public IPrintableConventionResult GetPrintableConvention(Guid conventionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var query = from convention in context.Conventions
                    where convention.ConventionId == conventionId
                    join place in context.Places on convention.ConventionId equals place.AssociatedConventionId
                    join session in context.Sessions on place.SessionId equals session.SessionId
                    join formation in context.Formations on session.FormationId equals formation.FormationId
                    join lieu in context.Lieux on session.LieuId equals lieu.Id 
                    select new { convention.ConventionNumber, convention.TypeConvention,Formation = formation.Nom, session.DateDebut, session.DuréeEnJour, Lieu = lieu.Nom};

                var conv = query.First();

                return new PrintableConventionResult()
                {
                    NumeroConvention = conv.ConventionNumber,
                    TypeConvention = conv.TypeConvention,
                    Formation = conv.Formation,
                    Lieu = conv.Lieu,
                    DateDebut = conv.DateDebut,
                    Durée = conv.DuréeEnJour
                };                    
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