namespace GestionFormation.Kernel
{
    public interface IEventHandler
    {
        
    }

    public interface IEventHandler<in T> : IEventHandler where T : IDomainEvent
    {
        void Handle(T @event);
    }
}