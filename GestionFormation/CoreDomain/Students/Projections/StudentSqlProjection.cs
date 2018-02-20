using System.Linq;
using GestionFormation.CoreDomain.Sessions.Events;
using GestionFormation.CoreDomain.Students.Events;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Students.Projections
{
    public class StudentSqlProjection : IProjectionHandler,
        IEventHandler<StudentCreated>, 
        IEventHandler<StudentUpdated>, 
        IEventHandler<StudentDeleted>        
    {
        public void Handle(StudentCreated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Students.Find(@event.AggregateId);
                if (entity == null)
                {
                    entity = new StudentSqlEntity();
                    context.Students.Add(entity);
                }

                entity.StudentId = @event.AggregateId;
                entity.Lastname = @event.Lastname;
                entity.Firstname = @event.Firstname;                

                context.SaveChanges();
            }
        }

        public void Handle(StudentUpdated @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity = context.Students.Find(@event.AggregateId);
                if (entity == null)
                    throw new EntityNotFoundException(@event.AggregateId, "Stagiaires");
                entity.Lastname = @event.Lastname;
                entity.Firstname = @event.Firstname;                
                context.SaveChanges();
            }
        }

        public void Handle(StudentDeleted @event)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var entity =  context.GetEntity<StudentSqlEntity>(@event.AggregateId);
                entity.Removed = true;
                context.SaveChanges();
            }
        }             
    }
}