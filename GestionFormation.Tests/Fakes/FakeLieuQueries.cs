using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Lieux.Queries;

namespace GestionFormation.Tests.Fakes
{
    public class FakeLieuQueries : ILieuQueries
    {
        private List<ILieuResult> _lieux = new List<ILieuResult>();

        public void Add(string nom, string addresse, int places)
        {
            _lieux.Add(new Result(Guid.NewGuid(), nom, addresse, places));
        }
        
        public IReadOnlyList<ILieuResult> GetAll()
        {
            return _lieux;
        }

        public Guid? GetLieu(string nom)
        {
            return _lieux.FirstOrDefault(a => a.Nom == nom)?.LieuId;
        }

        private class Result : ILieuResult
        {
            public Result(Guid lieuId, string nom, string addresse, int places)
            {
                LieuId = lieuId;
                Nom = nom;
                Addresse = addresse;
                Places = places;
            }
            public Guid LieuId { get;  }
            public string Nom { get; }
            public string Addresse { get;  }
            public int Places { get; }
        }
    }
}