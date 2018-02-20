using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<ISessionResult> GetAll(Guid trainingId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Sessions.Where(a => a.TrainingId == trainingId).ToList().Select(a => new SessionResult(a)).ToList();
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
                    select new {session, TrainingName = training.Name, TrainerFirstname = trainer.Firstname, TrainerLastname = trainer.Lastname, Location = location.Name};                                       

                return query.ToList().Select(a=>new CompleteSessionResult(a.session, a.TrainingName, a.Location, a.TrainerLastname, a.TrainerFirstname)).ToList();
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
                select new { session, TrainingName = training.Name, TrainerFirstname = trainer.Firstname, TrainerLastname = trainer.Lastname, Location = location.Name };

                var result = query.FirstOrDefault();
                return result == null ? null : new CompleteSessionResult(result.session, result.TrainingName, result.Location, result.TrainerLastname, result.TrainerFirstname);
            }
        }

        public IClosedSessionResult GetClosedSession(Guid sessionId)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                var query = from session in context.Sessions
                    where session.SessionId == sessionId
                    join training in context.Trainings on session.TrainingId equals training.TrainingId              
                    select new { TrainingName = training.Name, session.SessionStart, session.SurveyId, session.TimesheetId};

                var result = query.FirstOrDefault();
                return result == null ? null : new ClosedSessionResultResult(result.TrainingName, result.SessionStart, result.SurveyId, result.TimesheetId);
            }
        }
    }
}