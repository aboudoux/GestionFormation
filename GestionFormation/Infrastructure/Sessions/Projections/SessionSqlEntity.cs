using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.Infrastructure.Sessions.Projections
{
    [Table("Session")]
    public class SessionSqlEntity
    {
        [Key]
        public Guid SessionId { get; set; }
        public Guid TrainingId { get; set; }        
        public DateTime SessionStart { get; set; }
        public int Duration { get; set; }
        public int Seats { get; set; }
        public int ReservedSeats { get; set; }
        public Guid? LocationId { get; set; }
        public Guid? TrainerId { get; set; }

        public bool Canceled { get; set; }
        public string CancelReason { get; set; }
        public Guid? SurveyId { get; set; }
        public Guid? TimesheetId { get; set; }
    }
}