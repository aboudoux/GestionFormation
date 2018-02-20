using System;
using System.Data.Entity;
using GestionFormation.Infrastructure.Agreements.Projections;
using GestionFormation.Infrastructure.Companies.Projections;
using GestionFormation.Infrastructure.Contacts.Projections;
using GestionFormation.Infrastructure.Locations.Projections;
using GestionFormation.Infrastructure.Notifications.Projections;
using GestionFormation.Infrastructure.Seats.Projections;
using GestionFormation.Infrastructure.Sessions.Projections;
using GestionFormation.Infrastructure.Students.Projections;
using GestionFormation.Infrastructure.Trainers.Projections;
using GestionFormation.Infrastructure.Trainings.Projections;
using GestionFormation.Infrastructure.Users.Projections;

namespace GestionFormation.Infrastructure
{
    public class ProjectionContext : DbContext
    {
        public ProjectionContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<StudentSqlEntity> Students { get; set; }
        public DbSet<TrainingSqlEntity> Trainings { get; set; }
        public DbSet<SessionSqlEntity> Sessions { get; set; }
        public DbSet<TrainerSqlEntity> Trainers { get; set; }
        public DbSet<LocationSqlEntity> Locations { get; set; }
        public DbSet<SeatSqlentity> Seats { get; set; }
        public DbSet<CompanySqlEntity> Companies { get; set; }
        public DbSet<ContactSqlEntity> Contacts { get; set; }  
        public DbSet<AgreementSqlEntity> Agreements { get; set; }
        public DbSet<UserSqlEntity> Users { get; set; }
        public DbSet<NotificationSqlEntity> Notifications { get; set; }
        public DbSet<NotificationManagerSqlEntity> NotificationManagers { get; set; }

        public T GetEntity<T>(Guid id) where T : class
        {
            var entity = Set<T>().Find(id);
            if (entity == null)
                throw new EntityNotFoundException(id, typeof(T).Name);
            return entity;
        }
    }
}