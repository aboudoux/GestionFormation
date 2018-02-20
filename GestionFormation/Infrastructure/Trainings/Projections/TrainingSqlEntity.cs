using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.Infrastructure.Trainings.Projections
{
    [Table("Training")]
    public class TrainingSqlEntity
    {
        [Key]
        public Guid TrainingId { get; set; }
        public string Name { get; set; }
        public int Seats { get; set; }
    }
}