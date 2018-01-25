using GestionFormation.CoreDomain.Formations.Events;
using GestionFormation.CoreDomain.Sessions.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Projections
{
    public class SessionSqlProjection : IProjectionHandler,
        IEventHandler<SessionPlanned>, 
        IEventHandler<SessionUpdated>, 
        IEventHandler<SessionDeleted>,
        IEventHandler<SessionCanceled>, 
        IEventHandler<FormationDeleted>
    {
        public void Handle(SessionPlanned @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Sessions.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new SessionSqlEntity();
                    context.Sessions.Add(entity);
                }

                entity.FormationId = @event.FormationId;
                entity.SessionId = @event.AggregateId;
                entity.DateDebut = @event.DateDebut;
                entity.DuréeEnJour = @event.DuréeEnJour;
                entity.FormateurId = @event.FormateurId;
                entity.LieuId = @event.LieuId;
                entity.Places = @event.NbrPlaces;
                entity.PlacesReservées = 0;
                context.SaveChanges();
            }
        }

        public void Handle(SessionUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Sessions.Find(@event.AggregateId);
                if( entity == null )
                    throw new EntityNotFoundException(@event.AggregateId, "Session");

                entity.FormationId = @event.FormationId;
                entity.SessionId = @event.AggregateId;
                entity.DateDebut = @event.DateDebut;
                entity.DuréeEnJour = @event.DuréeEnJour;
                entity.FormateurId = @event.FormateurId;
                entity.LieuId = @event.LieuId;
                entity.Places = @event.NbrPlaces;                
                context.SaveChanges();
            }
        }

        public void Handle(SessionDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = new SessionSqlEntity(){ SessionId = @event.AggregateId};
                context.Sessions.Attach(entity);
                context.Sessions.Remove(entity);
                context.SaveChanges();
            }
        }

        public void Handle(SessionCanceled @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Sessions.Find(@event.AggregateId);
                if (entity == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Session");
                entity.Annulé = true;
                entity.RaisonAnnulation = @event.Raison;
                context.SaveChanges();
            }
        }

        public void Handle(FormationDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                context.Database.ExecuteSqlCommand($"DELETE FROM dbo.SESSION WHERE FormationId = '{@event.AggregateId}'");
            }
        }
    }
}
