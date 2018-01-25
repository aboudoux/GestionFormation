using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Sessions.Projections;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Sessions.Queries
{
    public class SessionSqlQueries : ISessionQueries, IRuntimeDependency
    {
        public IReadOnlyList<ISessionResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Sessions.ToList().Select(a => new SessionResult(a)).ToList();
            }
        }

        public IReadOnlyList<ISessionResult> GetAll(Guid formationId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Sessions.Where(a => a.FormationId == formationId).ToList().Select(a => new SessionResult(a)).ToList();
            }
        }

        public IReadOnlyList<ICompleteSessionResult> GetAllCompleteSession()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var query = from session in context.Sessions
                    join formation in context.Formations on session.FormationId equals formation.FormationId
                    join formateur in context.Formateurs on session.FormateurId equals formateur.FormateurId
                    join lieu in context.Lieux on session.LieuId equals lieu.Id 
                    select new {session, Formation = formation.Nom, Formateur = formateur.Prenom + " " + formateur.Nom, Lieu = lieu.Nom};                                       

                return query.ToList().Select(a=>new CompleteSessionResult(a.session, a.Formation, a.Formateur, a.Lieu)).ToList();
            }
        }

        public IReadOnlyList<string> GetAllLieux()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Lieux.Select(a => a.Nom).ToList();
            }
        }

        public ICompleteSessionResult GetSession(Guid sessionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var query = from session in context.Sessions
                            where session.SessionId == sessionId
                join formation in context.Formations on session.FormationId equals formation.FormationId
                join formateur in context.Formateurs on session.FormateurId equals formateur.FormateurId
                join lieu in context.Lieux on session.LieuId equals lieu.Id
                select new { session, Formation = formation.Nom, Formateur = formateur.Prenom + " " + formateur.Nom, Lieu = lieu.Nom };

                var result = query.FirstOrDefault();
                return result == null ? null : new CompleteSessionResult(result.session, result.Formation, result.Formateur, result.Lieu);
            }
        }
    }

    public class CompleteSessionResult : SessionResult, ICompleteSessionResult
    {       
        public CompleteSessionResult(SessionSqlEntity session, string formationName, string formateurName, string lieu): base(session)
        {
            Formation = formationName;
            Formateur = formateurName;
            Lieu = lieu;
        }
       
        public string Formation { get; set; }
        public string Formateur { get; set; }
        public string Lieu { get; set; }
    }
}