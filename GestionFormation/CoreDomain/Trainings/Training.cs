using System;
using GestionFormation.CoreDomain.Trainings.Events;
using GestionFormation.CoreDomain.Trainings.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainings
{
    public class Training : AggregateRootUpdatableAndDeletable<TrainingUpdated, TrainingDeleted>
    {
        private bool _disabled = false;

        public Training(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            base.AddPlayers(player);
            player.Add<TrainingDisabled>(e => _disabled = true);
        }

        public static Training Create(string name, int seats, int color)
        {            
            if(string.IsNullOrEmpty(name))
                throw new TrainingEmptyNameException();

            var training = new Training(new History());
            training.AggregateId = Guid.NewGuid();
            training.UncommitedEvents.Add(new TrainingCreated(training.AggregateId, 1, name, seats, color));
            return training;
        }

        public void Update(string name, int seats, int color)
        {
            if (string.IsNullOrEmpty(name))
                throw new TrainingEmptyNameException();

            Update(new TrainingUpdated(AggregateId, GetNextSequence(), name, seats, color));
        }

        public void Delete()
        {
            Delete(new TrainingDeleted(AggregateId, GetNextSequence()));            
        }

        public void Disable()
        {
            if(!_disabled)
                RaiseEvent(new TrainingDisabled(AggregateId, GetNextSequence()));
        }
    }
}
