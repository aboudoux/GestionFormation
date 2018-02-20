using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.Infrastructure.Contacts.Projections
{
    [Table("Contact")]
    public class ContactSqlEntity
    {
        [Key]
        public Guid ContactId { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public Guid CompanyId { get; set; }
        public bool Removed { get; set; }
    }
}