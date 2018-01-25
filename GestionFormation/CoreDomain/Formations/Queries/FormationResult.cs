using System;
using GestionFormation.CoreDomain.Formations.Projections;

namespace GestionFormation.CoreDomain.Formations.Queries
{
    public class FormationResult : IFormationResult
    {
        public FormationResult(FormationSqlEntity entity)
        {
            Id = entity.FormationId;
            Nom = entity.Nom;
            Places = entity.Places;
        }
        public Guid Id { get; }
        public string Nom { get; }
        public int Places { get; }
    }
}