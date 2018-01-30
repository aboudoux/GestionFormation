using System;
using System.Collections.Generic;
using System.Linq;

namespace GestionFormation.CoreDomain.Trainers
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

        public void Add(DateTime sessionStart, int duration)
        {            
            _periodList.Add(CreatePeriod(sessionStart, duration));
        }

        public CanUpdateResult CanUpdate(DateTime oldSessionStart, int oldDuration, DateTime newSessionStart, int newDuration)
        {
            var periodToUpdate = SearchPeriod(oldSessionStart, oldDuration);
            if (periodToUpdate == null)
                return CanUpdateResult.PeriodNotExists;

            _periodList.Remove(periodToUpdate);
            var result = IsFreeFor(newSessionStart, newDuration);
            _periodList.Add(periodToUpdate);
            return result ? CanUpdateResult.Ok : CanUpdateResult.NotFreeForNewPeriod;
        }

        public void Update(DateTime oldSessionStart, int oldDuration, DateTime newSessionStart, int newDuration)
        {
            var periodToUpdate = Remove(oldSessionStart, oldDuration);
            if (periodToUpdate == null)
                return;
            
            _periodList.Add(CreatePeriod(newSessionStart, newDuration));            
        }
       
        public Tuple<DateTime, DateTime> Remove(DateTime sessionStart, int duration)
        {
            var periodToRemove = SearchPeriod(sessionStart, duration);
            if (periodToRemove == null)
                return null;

            _periodList.Remove(periodToRemove);
            return periodToRemove;
        }

        public bool PeriodExists(DateTime sessionStart, int duration)
        {
            return SearchPeriod(sessionStart, duration) != null;
        }

        public bool IsFreeFor(DateTime sessionStart, int duration)
        {
            var sessionEnd = sessionStart.AddDays(duration - 1);
            if (_periodList.Any(a => sessionStart <= a.Item1 && sessionEnd >= a.Item1))
                return false;
            if (_periodList.Any(a => sessionStart >= a.Item1 && sessionEnd <= a.Item2))
                return false;
            if (_periodList.Any(a => sessionStart <= a.Item2 && sessionEnd >= a.Item2))
                return false;

            return true;
        }

        public bool Any()
        {
            return _periodList.Any();
        }

        private Tuple<DateTime, DateTime> SearchPeriod(DateTime sessionStart, int duration)
        {
            var oldPeriod = CreatePeriod(sessionStart, duration);
            var periodToUpdate = _periodList.FirstOrDefault(a => Equals(a, oldPeriod));
            return periodToUpdate;
        }

        private Tuple<DateTime, DateTime> CreatePeriod(DateTime sessionStart, int duration)
        {
            return Tuple.Create(sessionStart.Date, sessionStart.Date.AddDays(duration - 1));
        }
    }
}