using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.Infrastructure.Students.Projections
{
    [Table("Student")]
    public class StudentSqlEntity
    {
        [Key]
        public Guid StudentId { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public bool Removed { get; set; }
    }
}