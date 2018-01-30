using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Trainers.Projections
{
    [Table("Trainer")]
    public class TrainerSqlEntity
    {
        [Key]
        public Guid TrainerId { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }

    }
}