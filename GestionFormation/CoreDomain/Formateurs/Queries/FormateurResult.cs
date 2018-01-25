using System;
using GestionFormation.CoreDomain.Formateurs.Projections;

namespace GestionFormation.CoreDomain.Formateurs.Queries
{
    public class FormateurResult : IFormateurResult
    {
        public FormateurResult(FormateurSqlEntity entity)
        {
            Id = entity.FormateurId;
            Nom = entity.Nom;
            Prenom = entity.Prenom;
            Email = entity.Email;
        }
        public Guid Id { get; }
        public string Nom { get; }
        public string Prenom { get; }
        public string Email { get; }
    }
}