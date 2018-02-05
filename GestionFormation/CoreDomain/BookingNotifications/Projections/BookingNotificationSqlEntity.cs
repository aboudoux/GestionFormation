using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionFormation.CoreDomain.Users;

namespace GestionFormation.CoreDomain.BookingNotifications.Projections
{
    [Table("BookingNotification")]
    public class BookingNotificationSqlEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? SeatId { get; set; }
        public Guid SessionId { get; set; }
        public Guid CompanyId { get; set; }

        public string Label { get; set; }
        public UserRole AffectedRole { get; set; }
        public Guid? AgreementId { get; set; }
        public BookingNotificationType ReminderType { get; set; }
    }
}