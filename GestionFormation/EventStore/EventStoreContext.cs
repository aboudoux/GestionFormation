using System.Data.Entity;

namespace GestionFormation.EventStore
{
    public class EventStoreContext : DbContext
    {
        public EventStoreContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<DbEvent> Events { get; set; }
    }
}