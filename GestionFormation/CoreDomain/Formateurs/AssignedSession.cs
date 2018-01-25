using System;
using System.Collections.Generic;
using System.Linq;

namespace GestionFormation.CoreDomain.Formateurs
{
    public class AssignedSession
    {
        public enum CanUpdateResult
        {
            Ok = 0,
            PeriodNotExists = 1,
            NotFreeForNewPeriod = 2
        }

        private readonly List<Tuple<DateTime, DateTime>> _periodList = new List<Tuple<DateTime, DateTime>>();

        public void Add(DateTime dateDebut, int dur�e)
        {            
            _periodList.Add(CreatePeriod(dateDebut, dur�e));
        }

        public CanUpdateResult CanUpdate(DateTime oldDateDebut, int oldDur�e, DateTime newDateDebut, int newDur�e)
        {
            var periodToUpdate = SearchPeriod(oldDateDebut, oldDur�e);
            if (periodToUpdate == null)
                return CanUpdateResult.PeriodNotExists;

            _periodList.Remove(periodToUpdate);
            var result = IsFreeFor(newDateDebut, newDur�e);
            _periodList.Add(periodToUpdate);
            return result ? CanUpdateResult.Ok : CanUpdateResult.NotFreeForNewPeriod;
        }

        public void Update(DateTime oldDateDebut, int oldDur�e, DateTime newDateDebut, int newDur�e)
        {
            var periodToUpdate = Remove(oldDateDebut, oldDur�e);
            if (periodToUpdate == null)
                return;
            
            _periodList.Add(CreatePeriod(newDateDebut, newDur�e));            
        }
       
        public Tuple<DateTime, DateTime> Remove(DateTime debut, int dur�e)
        {
            var periodToRemove = SearchPeriod(debut, dur�e);
            if (periodToRemove == null)
                return null;

            _periodList.Remove(periodToRemove);
            return periodToRemove;
        }

        public bool PeriodExists(DateTime debut, int dur�e)
        {
            return SearchPeriod(debut, dur�e) != null;
        }

        public bool IsFreeFor(DateTime dateDebut, int dur�e)
        {
            var dateFin = dateDebut.AddDays(dur�e - 1);
            if (_periodList.Any(a => dateDebut <= a.Item1 && dateFin >= a.Item1))
                return false;
            if (_periodList.Any(a => dateDebut >= a.Item1 && dateFin <= a.Item2))
                return false;
            if (_periodList.Any(a => dateDebut <= a.Item2 && dateFin >= a.Item2))
                return false;

            return true;
        }

        public bool Any()
        {
            return _periodList.Any();
        }

        private Tuple<DateTime, DateTime> SearchPeriod(DateTime debut, int dur�e)
        {
            var oldPeriod = CreatePeriod(debut, dur�e);
            var periodToUpdate = _periodList.FirstOrDefault(a => Equals(a, oldPeriod));
            return periodToUpdate;
        }

        private Tuple<DateTime, DateTime> CreatePeriod(DateTime debut, int dur�e)
        {
            return Tuple.Create(debut.Date, debut.Date.AddDays(dur�e - 1));
        }
    }
}