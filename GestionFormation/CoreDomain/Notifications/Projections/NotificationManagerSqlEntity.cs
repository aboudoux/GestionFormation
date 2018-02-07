using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFormation.CoreDomain.Notifications.Projections
{
    [Table("NotificationManager")]
    public class NotificationManagerSqlEntity
    {
        [Key, Column(Order = 0)]
        public Guid Id { get; set; }
        [Key, Column(Order = 1)]
        public Guid SessionId { get; set; }
    }
}