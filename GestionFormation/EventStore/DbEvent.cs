using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionFormation.Kernel;

namespace GestionFormation.EventStore
{
    [Table("Event")]
    public class DbEvent
    {
        // constructeur vide pour entity framework
        public DbEvent()
        {            
        }

        public DbEvent(IDomainEvent @event, IEventSerializer eventSerializer, IEventStamping eventStamping)
        {
            if (eventSerializer == null) throw new ArgumentNullException(nameof(eventSerializer));
            if (eventStamping == null) throw new ArgumentNullException(nameof(eventStamping));

            var eventType = @event.GetType();
            AggregateId = @event.AggregateId;
            Sequence = @event.Sequence;
            EventName = eventType.Name;
            TimeStamp = eventStamping.GetDateTime();
            User = eventStamping.GetCurrentUser();
            Data = eventSerializer.Serialize(@event);
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        
        public Guid AggregateId { get; set; }        
        public int Sequence { get; set; }
        public string Data { get; set; }
        public string EventName { get; set; }
        public string User { get; set; }
        public DateTime TimeStamp { get; set; }       
    }
}