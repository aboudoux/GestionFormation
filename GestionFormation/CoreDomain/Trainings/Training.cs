using System;
using GestionFormation.CoreDomain.Trainings.Events;
using GestionFormation.CoreDomain.Trainings.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainings
{
    public class Training : AggregateRootUpdatableAndDeletable<TrainingUpdated, TrainingDeleted>
    {
        public Training(History history) : base(history)
        {
        }
     
        public static Training Create(string name, int seats)
        {
            if(string.IsNullOrEmpty(name))
                throw new TrainingEmptyNameException();

            var training = new Training(new History());
            training.AggregateId = Guid.NewGuid();
            training.UncommitedEvents.Add(new TrainingCreated(training.AggregateId, 1, name, seats));
            return training;
        }

        public void Update(string name, int seats)
        {
            if (string.IsNullOrEmpty(name))
                throw new TrainingEmptyNameException();

            Update(new TrainingUpdated(AggregateId, GetNextSequence(), name, seats));
        }

        public void Delete()
        {
            Delete(new TrainingDeleted(AggregateId, GetNextSequence()));            
        }
    }
}
