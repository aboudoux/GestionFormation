using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Trainings.Queries;
using GestionFormation.EventStore;
using GestionFormation.Kernel;

namespace GestionFormation.Infrastructure.Trainings.Queries
{
    public class TrainingSqlQueries : ITrainingQueries, IRuntimeDependency
    {
        public IReadOnlyList<ITrainingResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Trainings.ToList().Select(entity => new TrainingResult(entity)).ToList();
            }
        }

        public Guid? GetTrainingId(string trainingName)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Trainings.FirstOrDefault(a=>a.Name.ToLower()==trainingName.ToLower())?.TrainingId;
            }
        }        
    }
}