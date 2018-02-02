using System;
using System.Linq;
using GestionFormation.CoreDomain.Agreements.Events;
using GestionFormation.CoreDomain.Companies.Projections;
using GestionFormation.CoreDomain.Seats;
using GestionFormation.CoreDomain.Seats.Events;
using GestionFormation.CoreDomain.Seats.Projections;
using GestionFormation.CoreDomain.Students.Projections;
using GestionFormation.CoreDomain.Users;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Reminders.Projections
{
    public class RappelSqlProjections : IProjectionHandler,
        IEventHandler<SeatCreated>,
        IEventHandler<SeatRefused>,
        IEventHandler<SeatValided>,
        IEventHandler<SeatCanceled>,
        IEventHandler<AgreementCreated>,
        IEventHandler<AgreementRevoked>,
        IEventHandler<AgreementSigned>,
        IEventHandler<AgreementAssociated>
    {
        public void Handle(SeatCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Reminders.FirstOrDefault(a => a.SeatId == @event.AggregateId);
                if (entity == null)
                {
                    entity = new ReminderSqlEntity();
                    context.Reminders.Add(entity);
                }

                var stagiaire = context.GetEntity<StudentSqlEntity>(@event.StudentId);

                entity.SeatId = @event.AggregateId;
                entity.SessionId = @event.SessionId;
                entity.CompanyId = @event.CompanyId;
                entity.Label = $"Place de {stagiaire.Lastname} {stagiaire.Firstname} à valider.";
                entity.AffectedRole = UserRole.Manager;                
                entity.ReminderType = RappelType.PlaceToValidate;

                context.SaveChanges();
            }
        }

        public void Handle(SeatRefused @event)
        {
            RemovePlaceRappel(@event.AggregateId);
        }

        public void Handle(SeatValided @event)
        {
            RemovePlaceRappel(@event.AggregateId);

            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                AddConventionToCreate(context, @event.AggregateId);
            }
        }

        public void Handle(SeatCanceled @event)
        {
            RemovePlaceRappel(@event.AggregateId);

            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var place = context.GetEntity<SeatSqlentity>(@event.AggregateId);
                if (place.AssociatedAgreementId.HasValue && context.Seats.Where(a => a.AssociatedAgreementId == place.AssociatedAgreementId && a.SeatId != @event.AggregateId).All(a => a.Status != SeatStatus.Valid)) {                     
                    RemoveRappel(place.SessionId, place.CompanyId);
                }
            }
        }

        private void AddConventionToCreate(ProjectionContext context, Guid placeId)
        {
            var place = context.GetEntity<SeatSqlentity>(placeId);
            var societe = context.GetEntity<CompanySqlEntity>(place.CompanyId);
            if (!context.Reminders.Any(a => a.CompanyId == societe.CompanyId && a.SessionId == place.SessionId))
            {
                var entity = new ReminderSqlEntity();

                entity.SessionId = place.SessionId;
                entity.CompanyId = societe.CompanyId;

                entity.ReminderType = RappelType.ConventionToCreate;                
                entity.AffectedRole = UserRole.Operator;
                entity.Label = $"{societe.Name} - Convention à créer";

                context.Reminders.Add(entity);
                context.SaveChanges();
            }
        }

        public void Handle(AgreementCreated @event)
        {                        
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {                
                var entity = new ReminderSqlEntity();
                entity.AgreementId = @event.AggregateId;                
                entity.ReminderType = RappelType.ConventionToSign;
                entity.AffectedRole = UserRole.Operator;
                entity.Label = $"{@event.Agreement} - Convention à retourner signée";
                context.Reminders.Add(entity);
                context.SaveChanges();
            }
        }

        public void Handle(AgreementRevoked @event)
        {
            RemoveConventionRappel(@event.AggregateId);
            
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var placeEntity = context.Seats.FirstOrDefault(a => a.AssociatedAgreementId == @event.AggregateId && a.Status == SeatStatus.Valid);
                if (placeEntity != null)
                {
                    RemovePlaceRappel(placeEntity.SeatId);
                    var societe = context.GetEntity<CompanySqlEntity>(placeEntity.CompanyId);

                    var entity = new ReminderSqlEntity();
                    
                    entity.SessionId= placeEntity.SessionId;                    
                    entity.ReminderType = RappelType.ConventionToCreate;
                    entity.CompanyId = placeEntity.CompanyId;
                    entity.AffectedRole = UserRole.Operator;
                    entity.Label = $"{societe.Name} - Convention à créer";

                    context.Reminders.Add(entity);
                    context.SaveChanges();
                }
            }
        }

        public void Handle(AgreementSigned @event)
        {
            RemoveConventionRappel(@event.AggregateId);
        }

        public void Handle(AgreementAssociated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var place = context.GetEntity<SeatSqlentity>(@event.AggregateId);
                RemoveRappel(place.SessionId, place.CompanyId);
            }
        }

        private static void RemovePlaceRappel(Guid placeId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                context.Database.ExecuteSqlCommand($"DELETE FROM Rappel WHERE PlaceId = '{placeId}'");
            }
        }

        private static void RemoveConventionRappel(Guid conventionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                context.Database.ExecuteSqlCommand($"DELETE FROM Rappel WHERE ConventionId = '{conventionId}'");
            }
        }

        private static void RemoveRappel(Guid sessionId, Guid societeId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                context.Database.ExecuteSqlCommand($"DELETE FROM Rappel WHERE SessionId = '{sessionId}' AND SocieteId = '{societeId}'");
            }
        }
    }

    public enum RappelType
    {
        PlaceToValidate = 1,
        ConventionToCreate = 2,
        ConventionToSign = 3
    }
}
