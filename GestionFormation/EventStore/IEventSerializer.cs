namespace GestionFormation.EventStore
{
    public interface IEventSerializer
    {
        string Serialize(object data);
        T Deserialize<T>(string data);
    }
}