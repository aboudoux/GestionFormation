using System;
using System.Collections.Generic;
using GestionFormation.CoreDomain.Sessions.Queries;

namespace GestionFormation.Tests.Fakes
{
    public class FakeSessionQueries : ISessionQueries
    {
        private List<ISessionResult> _session = new List<ISessionResult>();

        public void AddSession(Guid formationId, Guid sessionId, DateTime dateDebut, int durée, Guid? lieuId, Guid? FormateurId)
        {
            var sessionResult = new FakeSessionResult();
            sessionResult.FormationId = formationId;
            sessionResult.SessionId = sessionId;
            sessionResult.DateDebut = dateDebut;
            sessionResult.Durée = durée;
            sessionResult.FormateurId = FormateurId;
            sessionResult.LieuId = lieuId;
            _session.Add(sessionResult);
        }

        public IReadOnlyList<ISessionResult> GetAll()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<ISessionResult> GetAll(Guid formationId)
        {
            return _session;
        }

        public IReadOnlyList<ICompleteSessionResult> GetAllCompleteSession()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<string> GetAllLieux()
        {
            throw new NotImplementedException();
        }

        public ICompleteSessionResult GetSession(Guid sessionId)
        {
            throw new NotImplementedException();
        }

        private class FakeSessionResult : ISessionResult
        {
            public Guid SessionId { get; set; }
            public Guid FormationId { get; set; }
            public DateTime DateDebut { get; set; }
            public int Durée { get; set; }
            public int Places { get; set; }
            public int PlacesReservées { get; set; }
            public Guid? LieuId { get; set; }
            public Guid? FormateurId { get; set; }
            public bool Annulé { get; set; }
            public string RaisonAnnulation { get; set; }
        }
    }
}