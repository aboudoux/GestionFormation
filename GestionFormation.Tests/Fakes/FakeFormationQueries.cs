using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Formations.Queries;

namespace GestionFormation.Tests.Fakes
{
    public class FakeFormationQueries : IFormationQueries
    {
        private List<IFormationResult> _formations = new List<IFormationResult>();

        public void AddFormation(string formation, int places)
        {
            _formations.Add(new Formation(Guid.NewGuid(), formation, places ));
        }

        public void AddFormation(Guid id, string formation, int places)
        {
            _formations.Add(new Formation(id, formation, places));
        }

        public IReadOnlyList<IFormationResult> GetAll()
        {
            return _formations;
        }

        public Guid? GetFormation(string formationName)
        {
            return _formations.FirstOrDefault(a=>a.Nom.ToLower() == formationName.ToLower())?.Id;
        }

        private class  Formation : IFormationResult
        {
            public Formation(Guid id, string nom, int places)
            {
                Id = id;
                Nom = nom;
                Places = places;
            }

            public Guid Id { get; }
            public string Nom { get; }
            public int Places { get; }
        }
    }
}