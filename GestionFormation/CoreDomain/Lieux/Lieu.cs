using System;
using GestionFormation.Applications.Lieux.Exceptions;
using GestionFormation.CoreDomain.Formateurs;
using GestionFormation.CoreDomain.Formateurs.Exceptions;
using GestionFormation.CoreDomain.Lieux.Events;
using GestionFormation.CoreDomain.Lieux.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Lieux
{
    public class Lieu : AggregateRootUpdatableAndDeletable<LieuUpdated, LieuDeleted>, IAssignable
    {
        private readonly AssignedSession _assignedSession = new AssignedSession();
        public Lieu(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            base.AddPlayers(player);
            player.Add<LieuAssigned>(e => _assignedSession.Add(e.DebutSession, e.Durée))
                .Add<LieuReassigned>(e => _assignedSession.Update(e.OldDateDebut, e.OldDurée, e.NewDateDebut, e.NewDurée))
                .Add<LieuUnassigned>(e => _assignedSession.Remove(e.DateDebut, e.Durée));
        }

        public static Lieu Create(string nom, string addresse, int places)
        {
            if(string.IsNullOrWhiteSpace(nom))
                throw new LieuWithEmptyNameException();

            var lieu = new Lieu(History.Empty);
            lieu.AggregateId = Guid.NewGuid();
            lieu.UncommitedEvents.Add(new LieuCreated(lieu.AggregateId, 1, nom, addresse, places));
            return lieu;
        }

        public void Update(string nom, string addresse, int places)
        {
            if (string.IsNullOrWhiteSpace(nom))
                throw new LieuWithEmptyNameException();

            Update(new LieuUpdated(AggregateId, GetNextSequence(), nom, addresse, places));
        }

        public void Delete()
        {
            if (_assignedSession.Any())
                throw new ForbiddenDeleteLieuException();

            Delete(new LieuDeleted(AggregateId, GetNextSequence()));
        }

        public void Assign(DateTime debutSession, int durée)
        {
            if (!_assignedSession.IsFreeFor(debutSession, durée))
                throw new LieuAlreadyAssignedException();

            RaiseEvent(new LieuAssigned(AggregateId, GetNextSequence(), debutSession, durée));
        }

        public void ChangeAssignation(DateTime oldDateDebut, int oldDurée, DateTime newDateDebut, int newDurée)
        {
            var canUpdate = _assignedSession.CanUpdate(oldDateDebut, oldDurée, newDateDebut, newDurée);
            switch (canUpdate)
            {
                case AssignedSession.CanUpdateResult.NotFreeForNewPeriod:
                    throw new LieuAlreadyAssignedException();
                case AssignedSession.CanUpdateResult.PeriodNotExists:
                    throw new PeriodDoNotExistsException();
            }

            RaiseEvent(new LieuReassigned(AggregateId, GetNextSequence(), oldDateDebut, oldDurée, newDateDebut, newDurée));
        }

        public void UnAssign(DateTime debut, int durée)
        {
            if (_assignedSession.PeriodExists(debut, durée))
            {
                RaiseEvent(new LieuUnassigned(AggregateId, GetNextSequence(), debut, durée));
            }
        }
    }
}
