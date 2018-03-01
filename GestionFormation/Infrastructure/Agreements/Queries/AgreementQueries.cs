using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Agreements.Queries;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure.Agreements.Projections;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure.Agreements.Queries
{
    public class AgreementQueries : IAgreementQueries, IRuntimeDependency
    {
        public IEnumerable<IAgreementResult> GetAll(Guid sessionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var query = from seats in context.Seats
                    where seats.SessionId == sessionId
                    join agreement in context.Agreements on seats.AssociatedAgreementId equals agreement.AgreementId
                    join companie in context.Companies on seats.CompanyId equals companie.CompanyId
                    join contact in context.Contacts on agreement.ContactId equals contact.ContactId                    
                    select new {Company = companie.Name, Contact = contact.Lastname, AgreementId = agreement.AgreementId, SeatId = seats.SeatId, AgreementNumber = agreement.AgreementNumber};

                return query.ToList().GroupBy(g => g.AgreementId, (key, a) =>
                {
                    var item = a.First();
                    return new AgreementResult(item.AgreementId, item.Company, item.Contact, item.AgreementNumber, a.Select(b => b.SeatId).ToList());
                });
            }
        }

        public IPrintableAgreementResult GetPrintableAgreement(Guid agreementId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var query = from agreement in context.Agreements
                    where agreement.AgreementId == agreementId
                    join seat in context.Seats on agreement.AgreementId equals seat.AssociatedAgreementId
                    join session in context.Sessions on seat.SessionId equals session.SessionId
                    join training in context.Trainings on session.TrainingId equals training.TrainingId
                    join location in context.Locations on session.LocationId equals location.Id 
                    select new { ConventionNumber = agreement.AgreementNumber, TypeConvention = agreement.AgreementTypeAgreement,Formation = training.Name, DateDebut = session.SessionStart, DuréeEnJour = session.Duration, Lieu = location.Name};

                var conv = query.First();

                return new PrintableAgreementResult()
                {                    
                    AgreementNumber = conv.ConventionNumber,
                    AgreementType = conv.TypeConvention,
                    Training = conv.Formation,
                    Location = conv.Lieu,
                    StartDate = conv.DateDebut,
                    Duration = conv.DuréeEnJour
                };                    
            }
        }

        public IAgreementDocumentResult GetAgreementDocument(Guid agreementId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {                
                var entity = context.GetEntity<AgreementSqlEntity>(agreementId);                
                return new AgreementDocumentResult(entity.DocumentId, entity.AgreementTypeAgreement);
            }
        }

        public long GetNextAgreementNumber()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Database.SqlQuery<long>("select next value for CompteurConvention").First();
            }
        }
    }
}