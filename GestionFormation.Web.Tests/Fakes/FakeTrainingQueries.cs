using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Trainings.Queries;

namespace GestionFormation.Tests.Fakes
{
    public class FakeTrainingQueries : ITrainingQueries
    {
        private List<ITrainingResult> _formations = new List<ITrainingResult>();

        public void AddFormation(string formation, int places)
        {
            _formations.Add(new Training(Guid.NewGuid(), formation, places ));
        }

        public IReadOnlyList<ITrainingResult> GetAll()
        {
            return _formations;
        }

        public Guid? GetTrainingId(string trainingName)
        {
            return _formations.FirstOrDefault(a=>a.Name.ToLower() == trainingName.ToLower())?.Id;
        }

        private class  Training : ITrainingResult
        {
            public Training(Guid id, string nom, int places)
            {
                Id = id;
                Name = nom;
                Seats = places;
            }

            public Guid Id { get; }
            public string Name { get; }
            public int Seats { get; }
        }
    }
}