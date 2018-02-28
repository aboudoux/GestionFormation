using System;
using GestionFormation.Applications.Trainings;

namespace DataMigration.Creators
{
    public class TrainingCreator : Creator
    {
        readonly Random _random = new Random();

        public TrainingCreator(ApplicationService applicationService) : base(applicationService)
        {
        }

        public void Create(string trainingName)
        {
            if(trainingName.IsEmpty()) return;
            if (Mapper.Exists(trainingName)) return;

            var training = App.Command<CreateTraining>().Execute(trainingName, 8, GetRandomColor());
            Mapper.Add(trainingName, training.AggregateId);            
        }

        private int GetRandomColor()
        {
            return (_random.Next(10, 255) << 24) | (_random.Next(10, 255) << 16) | (_random.Next(10, 255) << 8) | _random.Next(10, 255);
        }
    }
}