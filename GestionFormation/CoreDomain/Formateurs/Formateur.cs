using System;
using GestionFormation.CoreDomain.Formateurs.Events;
using GestionFormation.CoreDomain.Formateurs.Exceptions;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formateurs
{
    public class Formateur : AggregateRootUpdatableAndDeletable<FormateurUpdated, FormateurDeleted>, IAssignable
    {
        private readonly AssignedSession _assignedSession = new AssignedSession();
        private bool _isDisabled;

        public Formateur(History history) : base(history)
        {
        }

        protected override void AddPlayers(EventPlayer player)
        {
            base.AddPlayers(player);
            player.Add<FormateurAssigned>(e => _assignedSession.Add(e.DebutSession, e.Durée))
                .Add<FormateurReassigned>(e => _assignedSession.Update(e.OldDateDebut, e.OldDurée, e.NewDateDebut, e.NewDurée))
                .Add<FormateurUnassigned>(e => _assignedSession.Remove(e.DateDebut, e.Durée))
                .Add<FormateurDisabled>(e => _isDisabled = true);
        }

        public static Formateur Create(string nom, string prenom, string email)
        {
            if(string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(prenom))
                throw new FormateurNameException();

            var formateur = new Formateur(History.Empty);
            formateur.AggregateId = Guid.NewGuid();
            formateur.UncommitedEvents.Add(new FormateurCreated(formateur.AggregateId, 1, nom, prenom, email));
            return formateur;
        }

        public void Update(string nom, string prenom, string email)
        {
            if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(prenom))
                throw new FormateurNameException();

            Update(new FormateurUpdated(AggregateId, GetNextSequence(), nom, prenom, email));
        }

        public void Delete()
        {
            if(_assignedSession.Any())
                throw new ForbiddenDeleteFormateurException();                

            Delete(new FormateurDeleted(AggregateId, GetNextSequence()));
        }

        public void Assign(DateTime debutSession, int durée)
        {
            if (!_assignedSession.IsFreeFor(debutSession, durée))
                throw new FormateurAlreadyAssignedException();

            RaiseEvent(new FormateurAssigned(AggregateId, GetNextSequence(), debutSession, durée));
        }

        public void ChangeAssignation(DateTime oldDateDebut, int oldDurée, DateTime newDateDebut, int newDurée)
        {
            var canUpdate = _assignedSession.CanUpdate(oldDateDebut, oldDurée, newDateDebut, newDurée);
            switch (canUpdate)
            {
                case AssignedSession.CanUpdateResult.NotFreeForNewPeriod:
                    throw new FormateurAlreadyAssignedException();
                case AssignedSession.CanUpdateResult.PeriodNotExists:
                    throw new PeriodDoNotExistsException();
            }

            RaiseEvent(new FormateurReassigned(AggregateId, GetNextSequence(), oldDateDebut, oldDurée, newDateDebut, newDurée));
        }

        public void UnAssign(DateTime debut, int durée)
        {
            if (_assignedSession.PeriodExists(debut, durée))
            {
                RaiseEvent(new FormateurUnassigned(AggregateId, GetNextSequence(), debut, durée));
            }
        }

        public void Disable()
        {
            if (!_isDisabled)
            {
                RaiseEvent(new FormateurDisabled(AggregateId, GetNextSequence()));
            }
        }
    }
}
