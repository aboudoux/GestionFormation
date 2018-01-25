using System;
using System.Collections.Generic;
using System.Linq;
using GestionFormation.EventStore;
using GestionFormation.Infrastructure;
using GestionFormation.Kernel;

namespace GestionFormation.CoreDomain.Formations.Queries
{
    public class FormationSqlQueries : IFormationQueries, IRuntimeDependency
    {
        public IReadOnlyList<IFormationResult> GetAll()
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Formations.ToList().Select(entity => new FormationResult(entity)).ToList();
            }
        }

        public Guid? GetFormation(string formationName)
        {
            using (var context = new ProjectionContext(ConnectionString.Get()))
            {
                return context.Formations.FirstOrDefault(a=>a.Nom.ToLower()==formationName.ToLower())?.FormationId;
            }
        }        
    }
}