using System;
using GestionFormation.CoreDomain.Utilisateurs.Projections;

namespace GestionFormation.CoreDomain.Utilisateurs.Queries
{
    public class UtilisateurResult : IUtilisateurResult
    {
        public UtilisateurResult(UtilisateurSqlEntity entity)
        {
            Id = entity.Id;
            Login = entity.Login;
            Nom = entity.Nom;
            Prenom = entity.Prenom;
            Email = entity.Email;
            IsEnabled = entity.IsEnabled;
            Role = entity.Role;
        }
        public Guid Id { get; }
        public string Login { get; }
        public string Nom { get; }
        public string Prenom { get; }
        public bool IsEnabled { get; }
        public string Email { get; }
        public UtilisateurRole Role { get; }
    }
}