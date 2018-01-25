using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Utilisateurs.Projections
{
    [Table("Utilisateur")]
    public class UtilisateurSqlEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string EncryptedPassword { get; set; }        
        public bool IsEnabled { get; set; }
        public UtilisateurRole Role { get; set; }
    }
}