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

        public void Add(DateTime dateDebut, int durée)
        {            
            _periodList.Add(CreatePeriod(dateDebut, durée));
        }

        public CanUpdateResult CanUpdate(DateTime oldDateDebut, int oldDurée, DateTime newDateDebut, int newDurée)
        {
            var periodToUpdate = SearchPeriod(oldDateDebut, oldDurée);
            if (periodToUpdate == null)
                return CanUpdateResult.PeriodNotExists;

            _periodList.Remove(periodToUpdate);
            var result = IsFreeFor(newDateDebut, newDurée);
            _periodList.Add(periodToUpdate);
            return result ? CanUpdateResult.Ok : CanUpdateResult.NotFreeForNewPeriod;
        }

        public void Update(DateTime oldDateDebut, int oldDurée, DateTime newDateDebut, int newDurée)
        {
            var periodToUpdate = Remove(oldDateDebut, oldDurée);
            if (periodToUpdate == null)
                return;
            
            _periodList.Add(CreatePeriod(newDateDebut, newDurée));            
        }
       
        public Tuple<DateTime, DateTime> Remove(DateTime debut, int durée)
        {
            var periodToRemove = SearchPeriod(debut, durée);
            if (periodToRemove == null)
                return null;

            _periodList.Remove(periodToRemove);
            return periodToRemove;
        }

        public bool PeriodExists(DateTime debut, int durée)
        {
            return SearchPeriod(debut, durée) != null;
        }

        public bool IsFreeFor(DateTime dateDebut, int durée)
        {
            var dateFin = dateDebut.AddDays(durée - 1);
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

        private Tuple<DateTime, DateTime> SearchPeriod(DateTime debut, int durée)
        {
            var oldPeriod = CreatePeriod(debut, durée);
            var periodToUpdate = _periodList.FirstOrDefault(a => Equals(a, oldPeriod));
            return periodToUpdate;
        }

        private Tuple<DateTime, DateTime> CreatePeriod(DateTime debut, int durée)
        {
            return Tuple.Create(debut.Date, debut.Date.AddDays(durée - 1));
        }
    }
}