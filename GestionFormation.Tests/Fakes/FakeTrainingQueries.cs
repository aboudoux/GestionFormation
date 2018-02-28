using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Trainings.Queries;

namespace GestionFormation.Tests.Fakes
{
    public class FakeTrainingQueries : ITrainingQueries
    {
        private readonly List<ITrainingResult> _trainings = new List<ITrainingResult>();

        public void AddTraining(string training, int seats)
        {
            _trainings.Add(new Training(Guid.NewGuid(), training, seats ));
        }

        public void AddTraining(Guid id, string training, int seats)
        {
            _trainings.Add(new Training(id, training, seats));
        }

        public IReadOnlyList<ITrainingResult> GetAll()
        {
            return _trainings;
        }

        public Guid? GetTrainingId(string trainingName)
        {
            return _trainings.FirstOrDefault(a=>a.Name.ToLower() == trainingName.ToLower())?.Id;
        }

        private class  Training : ITrainingResult
        {
            public Training(Guid id, string name, int seats)
            {
                Id = id;
                Name = name;
                Seats = seats;
            }

            public Guid Id { get; }
            public string Name { get; }
            public int Seats { get; }
            public int Color { get; }
        }
    }
}