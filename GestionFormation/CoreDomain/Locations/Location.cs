using System;
using GestionFormation.Applications.Locations.Exceptions;
using GestionFormation.CoreDomain.Locations.Events;
using GestionFormation.CoreDomain.Locations.Exceptions;
using GestionFormation.CoreDomain.Trainers;
using GestionFormation.CoreDomain.Trainers.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Locations
{
    public class Location : AggregateRootUpdatableAndDeletable<LocationUpdated, LocationDeleted>, IAssignable
    {
        private readonly AssignedSession _assignedSession = new AssignedSession();
        private bool _disabled = false;
        public Location(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            base.AddPlayers(player);
            player.Add<LocationAssigned>(e => _assignedSession.Add(e.SessionStart, e.Duration))
                .Add<LocationReassigned>(e => _assignedSession.Update(e.OldSessionStart, e.OldDuration, e.NewSessionStart, e.NewDuration))
                .Add<LocationUnassigned>(e => _assignedSession.Remove(e.SessionStart, e.Duration))
                .Add<LocationDisabled>(a=>_disabled = true);
        }

        public static Location Create(string name, string address, int seats)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new LocationWithEmptyNameException();

            var location = new Location(History.Empty);
            location.AggregateId = Guid.NewGuid();
            location.UncommitedEvents.Add(new LocationCreated(location.AggregateId, 1, name, address, seats));
            return location;
        }

        public void Update(string name, string address, int seats)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new LocationWithEmptyNameException();

            Update(new LocationUpdated(AggregateId, GetNextSequence(), name, address, seats));
        }

        public void Delete()
        {
            if (_assignedSession.Any())
                throw new ForbiddenDeleteLocationException();

            Delete(new LocationDeleted(AggregateId, GetNextSequence()));
        }

        public void Assign(DateTime sessionStart, int duration)
        {
            if (!_assignedSession.IsFreeFor(sessionStart, duration))
                throw new LocationAlreadyAssignedException();

            RaiseEvent(new LocationAssigned(AggregateId, GetNextSequence(), sessionStart, duration));
        }

        public void ChangeAssignation(DateTime oldSessionStart, int oldDuration, DateTime newSessionStart, int newDuration)
        {
            var canUpdate = _assignedSession.CanUpdate(oldSessionStart, oldDuration, newSessionStart, newDuration);
            switch (canUpdate)
            {
                case AssignedSession.CanUpdateResult.NotFreeForNewPeriod:
                    throw new LocationAlreadyAssignedException();
                case AssignedSession.CanUpdateResult.PeriodNotExists:
                    throw new PeriodDoNotExistsException();
            }

            RaiseEvent(new LocationReassigned(AggregateId, GetNextSequence(), oldSessionStart, oldDuration, newSessionStart, newDuration));
        }

        public void UnAssign(DateTime sessionStart, int duration)
        {
            if (_assignedSession.PeriodExists(sessionStart, duration))
            {
                RaiseEvent(new LocationUnassigned(AggregateId, GetNextSequence(), sessionStart, duration));
            }
        }

        public void Disable()
        {
            if(!_disabled)
                RaiseEvent(new LocationDisabled(AggregateId, GetNextSequence()));
        }
    }
}
