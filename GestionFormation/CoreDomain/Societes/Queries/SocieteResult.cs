using System;
using GestionFormation.CoreDomain.Societes.Projections;

namespace GestionFormation.CoreDomain.Societes.Queries
{
    public class SocieteResult : ISocieteResult
    {
        public SocieteResult(SocieteSqlEntity entity)
        {
            SocieteId = entity.SocieteId;
            Nom = entity.Nom;
            Adresse = entity.Addresse;
            Codepostal = entity.CodePostal;
            Ville = entity.Ville;
        }
        public Guid SocieteId { get; }
        public string Nom { get; }
        public string Adresse { get; }
        public string Codepostal { get; }
        public string Ville { get; }
    }
}