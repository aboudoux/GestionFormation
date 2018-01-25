using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.CoreDomain.Stagiaires.Projections;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Stagiaires.Queries
{
    public class StagiaireSqlQueries : IStagiaireQueries, IRuntimeDependency
    {
        public IReadOnlyList<IStagiaireResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Stagiaires.ToList().Select(entity => new StagiaireResult(entity)).ToList();
            }
        }

        private class StagiaireResult : IStagiaireResult
        {
            public StagiaireResult(StagiaireSqlEntity entity)
            {
                Id = entity.StagiaireId;
                Nom = entity.Nom;
                Prenom = entity.Prenom;
            }

            public Guid Id { get; }
            public string Nom { get; }
            public string Prenom { get; }           
        }
    }
}