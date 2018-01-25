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
        public IEnumerable<ISessionResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Sessions.ToList().Select(a => new SessionResult(a)).ToList();
            }
        }

        public IEnumerable<ISessionResult> GetAll(Guid formationId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Sessions.Where(a => a.FormationId == formationId).ToList().Select(a => new SessionResult(a)).ToList();
            }
        }

        public IEnumerable<ICompleteSessionResult> GetAllCompleteSession()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var query = from session in context.Sessions
                    join formation in context.Formations on session.FormationId equals formation.FormationId
                    join formateur in context.Formateurs on session.FormateurId equals formateur.FormateurId
                    join lieu in context.Lieux on session.LieuId equals lieu.Id 
                    select new {session, Formation = formation.Nom, PrenomFormateur = formateur.Prenom, NomFormateur = formateur.Nom, Lieu = lieu.Nom};                                       

                return query.ToList().Select(a=>new CompleteSessionResult(a.session, a.Formation, a.Lieu, a.NomFormateur, a.PrenomFormateur)).ToList();
            }
        }

        public IEnumerable<string> GetAllLieux()
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
                select new { session, Formation = formation.Nom, PrenomFormateur = formateur.Prenom, NomFormateur = formateur.Nom, Lieu = lieu.Nom };

                var result = query.FirstOrDefault();
                return result == null ? null : new CompleteSessionResult(result.session, result.Formation, result.Lieu, result.NomFormateur, result.PrenomFormateur);
            }
        }
    }

    public class CompleteSessionResult : SessionResult, ICompleteSessionResult
    {       
        public CompleteSessionResult(SessionSqlEntity session, string formationName, string lieu, string nomFormateur, string prenomFormateur): base(session)
        {
            Formation = formationName;
            Formateur = new NomComplet(nomFormateur, prenomFormateur);
            Lieu = lieu;
        }
       
        public string Formation { get; }
        public NomComplet Formateur { get;  }
        public string Lieu { get;  }
    }
}