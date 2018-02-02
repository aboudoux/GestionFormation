using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Users.Projections
{
    [Table("User")]
    public class UserSqlEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string EncryptedPassword { get; set; }        
        public bool IsEnabled { get; set; }
        public UserRole Role { get; set; }
    }
}