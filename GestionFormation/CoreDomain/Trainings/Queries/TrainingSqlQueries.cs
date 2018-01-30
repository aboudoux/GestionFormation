using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainings.Queries
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