using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Students.Projections
{
    [Table("MissingStudent")]
    public class MissingStudentSqlEntity
    {
        [Key, Column(Order = 0)]
        public Guid StudentId { get; set; }
        [Key, Column(Order = 1)]
        public Guid SessionId { get; set; }
    }
}