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

        public IEnumerable<ISessionResult> GetAll(Guid TrainingId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Sessions.Where(a => a.TrainingId == TrainingId).ToList().Select(a => new SessionResult(a)).ToList();
            }
        }

        public IEnumerable<ICompleteSessionResult> GetAllCompleteSession()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var query = from session in context.Sessions
                    join training in context.Trainings on session.TrainingId equals training.TrainingId
                    join trainer in context.Trainers on session.TrainerId equals trainer.TrainerId
                    join location in context.Locations on session.LocationId equals location.Id 
                    select new {session, Formation = training.Name, PrenomFormateur = trainer.Firstname, NomFormateur = trainer.Lastname, Lieu = location.Name};                                       

                return query.ToList().Select(a=>new CompleteSessionResult(a.session, a.Formation, a.Lieu, a.NomFormateur, a.PrenomFormateur)).ToList();
            }
        }

        public IEnumerable<string> GetAllLocation()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Locations.Select(a => a.Name).ToList();
            }
        }

        public ICompleteSessionResult GetSession(Guid sessionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var query = from session in context.Sessions
                            where session.SessionId == sessionId
                join training in context.Trainings on session.TrainingId equals training.TrainingId
                join trainer in context.Trainers on session.TrainerId equals trainer.TrainerId
                join location in context.Locations on session.LocationId equals location.Id
                select new { session, Formation = training.Name, PrenomFormateur = trainer.Firstname, NomFormateur = trainer.Lastname, Lieu = location.Name };

                var result = query.FirstOrDefault();
                return result == null ? null : new CompleteSessionResult(result.session, result.Formation, result.Lieu, result.NomFormateur, result.PrenomFormateur);
            }
        }
    }

    public class CompleteSessionResult : SessionResult, ICompleteSessionResult
    {       
        public CompleteSessionResult(SessionSqlEntity session, string trainingName, string location, string trainerLastname, string trainerFirstname): base(session)
        {
            Training = trainingName;
            Trainer = new FullName(trainerLastname, trainerFirstname);
            Location = location;
        }
       
        public string Training { get; }
        public FullName Trainer { get;  }
        public string Location { get;  }
    }
}