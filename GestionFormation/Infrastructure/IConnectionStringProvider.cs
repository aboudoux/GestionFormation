namespace GestionFormation.Infrastructure
{
    public interface IConnectionStringProvider
    {
        void Write(string connectionString);
        string Read();
    }
}