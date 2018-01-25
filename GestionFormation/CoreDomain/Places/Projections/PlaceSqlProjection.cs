using System;
using GestionFormation.CoreDomain.Places.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Places.Projections
{
    public class PlaceSqlProjection : IProjectionHandler,
        IEventHandler<PlaceCreated>, 
        IEventHandler<PlaceCanceled>, 
        IEventHandler<PlaceRefused>, 
        IEventHandler<PlaceValided>,
        IEventHandler<ConventionAssociated>        
    {
        public void Handle(PlaceCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Places.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new PlaceSqlentity();
                    context.Places.Add(entity);
                }

                entity.PlaceId = @event.AggregateId;
                entity.SessionId = @event.SessionId;
                entity.SocieteId = @event.SocieteId;
                entity.StagiaireId = @event.StagiaireId;
                entity.Raison = "";
                entity.Status = PlaceStatus.AValider;
                context.SaveChanges();
            }
        }

        public void Handle(PlaceCanceled @event)
        {
            UpdateStatus(@event.AggregateId, PlaceStatus.Annulé, @event.Reason);
        }

        public void Handle(PlaceRefused @event)
        {
            UpdateStatus(@event.AggregateId, PlaceStatus.Refusé, @event.Raison);
        }

        public void Handle(PlaceValided @event)
        {
            UpdateStatus(@event.AggregateId, PlaceStatus.Validé);
        }      

        private void UpdateStatus(Guid placeId , PlaceStatus status, string reason = "")
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var place = context.Places.Find(placeId);
                if (place == null)
                    throw new EntityNotFoundException(placeId, "Place");
                place.Status = status;
                place.Raison = reason;
                context.SaveChanges();
            }
        }

        public void Handle(ConventionAssociated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var place = context.Places.Find(@event.AggregateId);
                if (place == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Place");
                place.AssociatedConventionId = @event.ConventionId;
                context.SaveChanges();
            }
        }       
    }
}
