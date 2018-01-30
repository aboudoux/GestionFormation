using System;
using GestionFormation.CoreDomain.Trainers.Events;
using GestionFormation.CoreDomain.Trainers.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Trainers
{
    public class Trainer : AggregateRootUpdatableAndDeletable<TrainerUpdated, TrainerDeleted>, IAssignable
    {
        private readonly AssignedSession _assignedSession = new AssignedSession();
        private bool _isDisabled;

        public Trainer(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            base.AddPlayers(player);
            player.Add<TrainerAssigned>(e => _assignedSession.Add(e.SessionStart, e.Duration))
                .Add<TrainerReassigned>(e => _assignedSession.Update(e.OldSessionStart, e.OldDuration, e.NewSessionStart, e.NewDuration))
                .Add<TrainerUnassigned>(e => _assignedSession.Remove(e.SessionStart, e.Duration))
                .Add<TrainerDisabled>(e => _isDisabled = true);
        }

        public static Trainer Create(string lastname, string firstname, string email)
        {
            if(string.IsNullOrWhiteSpace(lastname) || string.IsNullOrWhiteSpace(firstname))
                throw new TrainerNameException();

            var trainer = new Trainer(History.Empty);
            trainer.AggregateId = Guid.NewGuid();
            trainer.UncommitedEvents.Add(new TrainerCreated(trainer.AggregateId, 1, lastname, firstname, email));
            return trainer;
        }

        public void Update(string lastname, string firstname, string email)
        {
            if (string.IsNullOrWhiteSpace(lastname) || string.IsNullOrWhiteSpace(firstname))
                throw new TrainerNameException();

            Update(new TrainerUpdated(AggregateId, GetNextSequence(), lastname, firstname, email));
        }

        public void Delete()
        {
            if(_assignedSession.Any())
                throw new ForbiddenDeleteTrainerException();                

            Delete(new TrainerDeleted(AggregateId, GetNextSequence()));
        }

        public void Assign(DateTime sessionStart, int duration)
        {
            if (!_assignedSession.IsFreeFor(sessionStart, duration))
                throw new TrainerAlreadyAssignedException();

            RaiseEvent(new TrainerAssigned(AggregateId, GetNextSequence(), sessionStart, duration));
        }

        public void ChangeAssignation(DateTime oldSessionStart, int oldDuration, DateTime newSessionStart, int newDuration)
        {
            var canUpdate = _assignedSession.CanUpdate(oldSessionStart, oldDuration, newSessionStart, newDuration);
            switch (canUpdate)
            {
                case AssignedSession.CanUpdateResult.NotFreeForNewPeriod:
                    throw new TrainerAlreadyAssignedException();
                case AssignedSession.CanUpdateResult.PeriodNotExists:
                    throw new PeriodDoNotExistsException();
            }

            RaiseEvent(new TrainerReassigned(AggregateId, GetNextSequence(), oldSessionStart, oldDuration, newSessionStart, newDuration));
        }

        public void UnAssign(DateTime sessionStart, int duration)
        {
            if (_assignedSession.PeriodExists(sessionStart, duration))
            {
                RaiseEvent(new TrainerUnassigned(AggregateId, GetNextSequence(), sessionStart, duration));
            }
        }

        public void Disable()
        {
            if (!_isDisabled)
            {
                RaiseEvent(new TrainerDisabled(AggregateId, GetNextSequence()));
            }
        }
    }
}
