using System;
using System.Data.Entity;
using GestionFormation.CoreDomain.Agreements.Projections;
using GestionFormation.CoreDomain.Companies.Projections;
using GestionFormation.CoreDomain.Contacts.Projections;
using GestionFormation.CoreDomain.Locations.Projections;
using GestionFormation.CoreDomain.Rappels.Projections;
using GestionFormation.CoreDomain.Seats.Projections;
using GestionFormation.CoreDomain.Sessions.Projections;
using GestionFormation.CoreDomain.Stagiaires.Projections;
using GestionFormation.CoreDomain.Trainers.Projections;
using GestionFormation.CoreDomain.Trainings.Projections;
using GestionFormation.CoreDomain.Utilisateurs.Projections;

namespace GestionFormation.Infrastructure
{
    public class ProjectionContext : DbContext
    {
        public ProjectionContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<StagiaireSqlEntity> Stagiaires { get; set; }
        public DbSet<TrainingSqlEntity> Trainings { get; set; }
        public DbSet<SessionSqlEntity> Sessions { get; set; }
        public DbSet<TrainerSqlEntity> Trainers { get; set; }
        public DbSet<LocationSqlEntity> Locations { get; set; }
        public DbSet<SeatSqlentity> Seats { get; set; }
        public DbSet<CompanySqlEntity> Companies { get; set; }
        public DbSet<ContactSqlEntity> Contacts { get; set; }  
        public DbSet<AgreementSqlEntity> Agreements { get; set; }
        public DbSet<UtilisateurSqlEntity> Utilisateurs { get; set; }
        public DbSet<RappelSqlEntity> Rappels { get; set; }


        public T GetEntity<T>(Guid id) where T : class
        {
            var entity = Set<T>().Find(id);
            if (entity == null)
                throw new EntityNotFoundException(id, typeof(T).Name);
            return entity;
        }
    }
}