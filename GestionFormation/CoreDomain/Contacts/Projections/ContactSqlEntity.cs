using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Contacts.Projections
{
    [Table("Contact")]
    public class ContactSqlEntity
    {
        [Key]
        public Guid ContactId { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public Guid SocieteId { get; set; }
    }
}