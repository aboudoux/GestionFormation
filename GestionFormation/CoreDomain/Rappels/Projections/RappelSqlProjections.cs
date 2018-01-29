using System;
using System.Linq;
using GestionFormation.CoreDomain.Conventions;
using GestionFormation.CoreDomain.Conventions.Events;
using GestionFormation.CoreDomain.Places;
using GestionFormation.CoreDomain.Places.Events;
using GestionFormation.CoreDomain.Places.Projections;
using GestionFormation.CoreDomain.Sessions;
using GestionFormation.CoreDomain.Sessions.Projections;
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
                var entity = context.Rappels.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new RappelSqlEntity();
                    context.Rappels.Add(entity);
                }

                var stagiaire = context.GetEntity<StagiaireSqlEntity>(@event.StagiaireId);

                entity.Id = @event.AggregateId;
                entity.Label = $"Place de {stagiaire.Nom} {stagiaire.Prenom} à valider.";
                entity.AffectedRole = UtilisateurRole.GestionnaireFormation;
                entity.AggregateType = typeof(Session).Name;
                entity.AggregateId = @event.SessionId;
                entity.RappelType = RappelType.PlaceToValidate;

                context.SaveChanges();
            }
        }

        public void Handle(PlaceRefused @event)
        {
            RemoveRappel(@event.AggregateId);
        }

        public void Handle(PlaceValided @event)
        {
            RemoveRappel(@event.AggregateId);

            using (var context = new ProjectionContext(ConnectionString.Get()))
            {            
                var place = context.GetEntity<PlaceSqlentity>(@event.AggregateId);
                var societe = context.GetEntity<SocieteSqlEntity>(place.SocieteId);
                if (!context.Rappels.Any(a => a.SocieteId == societe.SocieteId && a.AggregateId == place.SessionId))
                {
                    var session = context.GetEntity<SessionSqlEntity>(place.SessionId);
                    var entity = new RappelSqlEntity();
                    entity.Id = @event.AggregateId;
                    entity.AggregateId = place.SessionId;
                    entity.AggregateType = typeof(Session).Name;
                    entity.RappelType = RappelType.ConventionToCreate;
                    entity.SocieteId = societe.SocieteId;
                    entity.AffectedRole = UtilisateurRole.ServiceFormation;                    
                    entity.Label = $"{societe.Nom} - Convention à créer";

                    context.Rappels.Add(entity);
                    context.SaveChanges();
                }
            }
        }

        public void Handle(PlaceCanceled @event)
        {
            RemoveRappel(@event.AggregateId);

            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var place = context.Places.Find(@event.AggregateId);
                if (place.AssociatedConventionId.HasValue && context.Places.Where(a=>a.AssociatedConventionId == place.AssociatedConventionId && a.PlaceId != @event.AggregateId).All(a=> a.Status != PlaceStatus.Validé))
                    RemoveRappel(place.AssociatedConventionId.Value);
            }
        }

        public void Handle(ConventionCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new RappelSqlEntity();
                entity.Id = @event.AggregateId;
                entity.AggregateId = @event.AggregateId;
                entity.AggregateType = typeof(Convention).Name;
                entity.RappelType = RappelType.ConventionToSign;
                entity.AffectedRole = UtilisateurRole.ServiceFormation;
                entity.Label = $"{@event.Convention} - Convention à retourner signée";
                context.Rappels.Add(entity);
                context.SaveChanges();
            }
        }

        public void Handle(ConventionRevoked @event)
        {            
            RemoveRappel(@event.AggregateId);
            
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var placeEntity = context.Places.FirstOrDefault(a => a.AssociatedConventionId == @event.AggregateId && a.Status == PlaceStatus.Validé);
                if (placeEntity != null)
                {
                    RemoveRappel(placeEntity.PlaceId);
                    var societe = context.GetEntity<SocieteSqlEntity>(placeEntity.SocieteId);

                    var entity = new RappelSqlEntity();
                    entity.Id = @event.AggregateId;
                    entity.AggregateId = placeEntity.SessionId;
                    entity.AggregateType = typeof(Session).Name;
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
            RemoveRappel(@event.AggregateId);
        }

        public void Handle(ConventionAssociated @event)
        {            
            RemoveRappel(@event.AggregateId);
        }

        private static void RemoveRappel(Guid id)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                context.Database.ExecuteSqlCommand($"DELETE FROM Rappel WHERE Id = '{id}'");
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
