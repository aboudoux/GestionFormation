using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Trainers.Queries;

namespace GestionFormation.Tests.Fakes
{
    public class FakeTrainerQueries : ITrainerQueries
    {
        private readonly List<ITrainerResult> _trainerResut = new List<ITrainerResult>();

        public void AddTrainer(string lastname, string firstname, string email = null, Guid? trainerId = null)
        {
            _trainerResut.Add(new Result(trainerId ??  Guid.NewGuid(), lastname, firstname, email));
        }

        public IReadOnlyList<ITrainerResult> GetAll()
        {
            return _trainerResut;
        }

        public Guid? GetTrainer(string lastname, string firstname)
        {
            return _trainerResut.FirstOrDefault(a=>a.Firstname.ToLower() == firstname.ToLower() && a.Lastname.ToLower() == lastname.ToLower())?.Id;
        }

        private class Result : ITrainerResult
        {
            public Result(Guid id, string lastname, string firstname, string email)
            {
                Id = id;
                Lastname = lastname;
                Firstname = firstname;
                Email = email;
            }

            public Guid Id { get; }
            public string Lastname { get; }
            public string Firstname { get; }
            public string Email { get; }
        }
    }
}