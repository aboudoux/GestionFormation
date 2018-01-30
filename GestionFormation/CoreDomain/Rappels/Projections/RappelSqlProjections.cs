using System;
using System.Linq;
using GestionFormation.CoreDomain.Conventions;
using GestionFormation.CoreDomain.Conventions.Events;
using GestionFormation.CoreDomain.Places;
using GestionFormation.CoreDomain.Places.Events;
using GestionFormation.CoreDomain.Places.Projections;
using GestionFormation.CoreDomain.Societes.Projections;
using GestionFormation.CoreDomain.Stagiaires.Projections;
using GestionFormation.CoreDomain.Utilisateurs;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Rappels.Projections
{
    public class RappelSqlProjections : IProjectionHandler,
        IEventHandler<PlaceCreated>,
        IEventHandler<PlaceRefused>,
        IEventHandler<PlaceValided>,
        IEventHandler<PlaceCanceled>,
        IEventHandler<ConventionCreated>,
        IEventHandler<ConventionRevoked>,
        IEventHandler<ConventionSigned>,
        IEventHandler<ConventionAssociated>
    {
        public void Handle(PlaceCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Rappels.FirstOrDefault(a => a.PlaceId == @event.AggregateId);
                if (entity == null)
                {
                    entity = new RappelSqlEntity();
                    context.Rappels.Add(entity);
                }

                var stagiaire = context.GetEntity<StagiaireSqlEntity>(@event.StagiaireId);

                entity.PlaceId = @event.AggregateId;
                entity.SessionId = @event.SessionId;
                entity.SocieteId = @event.SocieteId;
                entity.Label = $"Place de {stagiaire.Nom} {stagiaire.Prenom} à valider.";
                entity.AffectedRole = UtilisateurRole.GestionnaireFormation;                
                entity.RappelType = RappelType.PlaceToValidate;

                context.SaveChanges();
            }
        }

        public void Handle(PlaceRefused @event)
        {
            RemovePlaceRappel(@event.AggregateId);
        }

        public void Handle(PlaceValided @event)
        {
            RemovePlaceRappel(@event.AggregateId);

            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                AddConventionToCreate(context, @event.AggregateId);
            }
        }

        public void Handle(PlaceCanceled @event)
        {
            RemovePlaceRappel(@event.AggregateId);

            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var place = context.GetEntity<PlaceSqlentity>(@event.AggregateId);
                if (place.AssociatedConventionId.HasValue && context.Places.Where(a => a.AssociatedConventionId == place.AssociatedConventionId && a.PlaceId != @event.AggregateId).All(a => a.Status != PlaceStatus.Validé)) {                     
                    RemoveRappel(place.SessionId, place.SocieteId);
                }
            }
        }

        private void AddConventionToCreate(ProjectionContext context, Guid placeId)
        {
            var place = context.GetEntity<PlaceSqlentity>(placeId);
            var societe = context.GetEntity<SocieteSqlEntity>(place.SocieteId);
            if (!context.Rappels.Any(a => a.SocieteId == societe.SocieteId && a.SessionId == place.SessionId))
            {
                var entity = new RappelSqlEntity();

                entity.SessionId = place.SessionId;
                entity.SocieteId = societe.SocieteId;

                entity.RappelType = RappelType.ConventionToCreate;                
                entity.AffectedRole = UtilisateurRole.ServiceFormation;
                entity.Label = $"{societe.Nom} - Convention à créer";

                context.Rappels.Add(entity);
                context.SaveChanges();
            }
        }

        public void Handle(ConventionCreated @event)
        {                        
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {                
                var entity = new RappelSqlEntity();
                entity.ConventionId = @event.AggregateId;                
                entity.RappelType = RappelType.ConventionToSign;
                entity.AffectedRole = UtilisateurRole.ServiceFormation;
                entity.Label = $"{@event.Convention} - Convention à retourner signée";
                context.Rappels.Add(entity);
                context.SaveChanges();
            }
        }

        public void Handle(ConventionRevoked @event)
        {
            RemoveConventionRappel(@event.AggregateId);
            
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var placeEntity = context.Places.FirstOrDefault(a => a.AssociatedConventionId == @event.AggregateId && a.Status == PlaceStatus.Validé);
                if (placeEntity != null)
                {
                    RemovePlaceRappel(placeEntity.PlaceId);
                    var societe = context.GetEntity<SocieteSqlEntity>(placeEntity.SocieteId);

                    var entity = new RappelSqlEntity();
                    
                    entity.SessionId= placeEntity.SessionId;                    
                    entity.RappelType = RappelType.ConventionToCreate;
                    entity.SocieteId = placeEntity.SocieteId;
                    entity.AffectedRole = UtilisateurRole.ServiceFormation;
                    entity.Label = $"{societe.Nom} - Convention à créer";

                    context.Rappels.Add(entity);
                    context.SaveChanges();
                }
            }
        }

        public void Handle(ConventionSigned @event)
        {
            RemoveConventionRappel(@event.AggregateId);
        }

        public void Handle(ConventionAssociated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var place = context.GetEntity<PlaceSqlentity>(@event.AggregateId);
                RemoveRappel(place.SessionId, place.SocieteId);
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
