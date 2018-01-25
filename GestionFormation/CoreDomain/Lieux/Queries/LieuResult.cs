using System;
using GestionFormation.CoreDomain.Lieux.Projections;

namespace GestionFormation.CoreDomain.Lieux.Queries
{
    public class LieuResult : ILieuResult
    {
        public LieuResult(LieuSqlEntity entity)
        {
            LieuId = entity.Id;
            Nom = entity.Nom;
            Addresse = entity.Addresse;
            Places = entity.Places;
        }
        public Guid LieuId { get; }
        public string Nom { get; }
        public string Addresse { get; }
        public int Places { get; }
    }
}