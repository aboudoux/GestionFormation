using System.Data.Entity;
using GestionFormation.CoreDomain.Contacts.Projections;
using GestionFormation.CoreDomain.Conventions.Projections;
using GestionFormation.CoreDomain.Formateurs.Projections;
using GestionFormation.CoreDomain.Formations.Projections;
using GestionFormation.CoreDomain.Lieux.Projections;
using GestionFormation.CoreDomain.Places.Projections;
using GestionFormation.CoreDomain.Rappels.Projections;
using GestionFormation.CoreDomain.Sessions.Projections;
using GestionFormation.CoreDomain.Societes.Projections;
using GestionFormation.CoreDomain.Stagiaires.Projections;
using GestionFormation.CoreDomain.Utilisateurs.Projections;

namespace GestionFormation.Infrastructure
{
    public class ProjectionContext : DbContext
    {
        public ProjectionContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {            
        }

        public DbSet<StagiaireSqlEntity> Stagiaires { get; set; }
        public DbSet<FormationSqlEntity> Formations { get; set; }
        public DbSet<SessionSqlEntity> Sessions { get; set; }
        public DbSet<FormateurSqlEntity> Formateurs { get; set; }
        public DbSet<LieuSqlEntity> Lieux { get; set; }
        public DbSet<PlaceSqlentity> Places { get; set; }
        public DbSet<SocieteSqlEntity> Societes { get; set; }
        public DbSet<ContactSqlEntity> Contacts { get; set; }  
        public DbSet<ConventionSqlEntity> Conventions { get; set; }
        public DbSet<UtilisateurSqlEntity> Utilisateurs { get; set; }
        public DbSet<RappelSqlEntity> Rappels { get; set; }
    }
}